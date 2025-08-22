#  Direcional API
API desenvolvida em **.NET 9** para gestão de clientes, apartamentos, vendas e reservas.
Inclui autenticação via **JWT**, documentação **Swagger** e **testes de integração** utilizando SQLit---
##  Tecnologias
- .NET 9
- Entity Framework Core
- SQL Server Express
- JWT Bearer Authentication
- FluentValidation
- Swagger / Swashbuckle
- xUnit + WebApplicationFactory (testes de integração)
---
##  Configuração do ambiente
### 1. Clonar o repositório
```
git clone https://github.com/seu-repo/direcional-api.git
cd direcional-api
```
### 2. Subir o ambiente com Docker
```
docker compose up -d --build
```
Isso sobe:
- API em http://localhost:8080
- SQL Server no container
### 3. Acessar Swagger
```
http://localhost:8080/swagger
```
---
## ■ Autenticação
A API usa **JWT Bearer Token**.
### Exemplo de login
```
curl -X POST http://localhost:8080/api/auth/login -H "Content-Type: application/json" -d '{"login```
Resposta esperada:
```
{
 "token": "eyJhbGciOiJIUzI1NiIsInR..."
}
```
Use esse token no Swagger (**Authorize**) ou nas chamadas HTTP:
```
Authorization: Bearer {token}
```
---
## ■ Endpoints principais
### Clientes
```
# Listar clientes
curl -X GET http://localhost:8080/api/clientes -H "Authorization: Bearer {token}"
# Criar cliente
curl -X POST http://localhost:8080/api/clientes -H "Authorization: Bearer {token}" -H "Content-Type: # Atualizar cliente
curl -X PUT http://localhost:8080/api/clientes/1 -H "Authorization: Bearer {token}" -H "Content-Type# Remover cliente
curl -X DELETE http://localhost:8080/api/clientes/1 -H "Authorization: Bearer {token}"
```
### Apartamentos
```
curl -X GET http://localhost:8080/api/apartamentos -H "Authorization: Bearer {token}"
```
### Vendas
```
curl -X POST http://localhost:8080/api/vendas -H "Authorization: Bearer {token}" -H "Content-Type: ap```
### Reservas
```
curl -X POST http://localhost:8080/api/reservas -H "Authorization: Bearer {token}" -H "Content-Type: ```
---
## ■ Testes
Os testes rodam com **SQLite in-memory**, sem depender do SQL Server.
### Executar os testes:
```
dotnet test
```
---
## ■ Estrutura do projeto
```
src/
 Direcional.Api # API principal
 Controllers # Endpoints REST
 Infra # DbContext, Migrations
 Services # Serviços (ex: JWT)
 Program.cs # Configuração da aplicação
 Direcional.Tests # Testes de integração
  Integration # WebApplicationFactory + testes
