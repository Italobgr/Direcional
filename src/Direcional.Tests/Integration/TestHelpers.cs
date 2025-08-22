using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Direcional.Tests.Integration;

public static class TestHelpers
{
    //-----------------------------------------
    // helper para autenticar requests
    //-----------------------------------------

    public static async Task<string> GetJwtAsync(HttpClient client)
    {
        var resp = await client.PostAsJsonAsync("/api/Auth/login", new { username = "corretor", password = "123" });
        var json = await resp.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        return json!["token"];
    }

    public static void WithBearer(this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
