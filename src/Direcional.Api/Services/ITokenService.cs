namespace Direcional.Api.Services;

public interface ITokenService
{
    string GeraToken(string username, string role);
}