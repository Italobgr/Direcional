using System.Text;
using Direcional.Api.Infra;
using Direcional.Api.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlServer"),
        sql => sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), null) // resiliência
    );
});


builder.Services
    .AddControllers()
    .AddNewtonsoftJson(); 

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>(); // pega validators do assembly

//
builder.Services.AddAuthorization();

var jwtKey = builder.Configuration["Jwt:Key"] 
             ?? throw new InvalidOperationException("JWT Key (Jwt:Key) não foi configurada");//
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddScoped<ITokenService, TokenService>();



// configiração do swagger pt 8080 dck
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Direcional API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autenticação JWT via header Authorization. Ex: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(); 


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
