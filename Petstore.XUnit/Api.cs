using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace PetstoreTests;

public static class Api
{
    public static readonly HttpClient Client = new HttpClient
    {
        BaseAddress = new Uri("https://petstore.swagger.io/v2/")
    };

    public static int Id => Random.Shared.Next(100000, 900000);
    public static string Username => "test_" + Guid.NewGuid().ToString("N")[..10];

    public static object Pet(int id, string name = "TestPet", string status = "available") =>
        new { id, name, photoUrls = new[] { "https://example.com/pet.jpg" }, status };

    public static object User(string username, int id) => new
    {
        id, username, firstName = "Test", lastName = "User",
        email = username + "@example.com", password = "123456",
        phone = "1234567890", userStatus = 1
    };

    public static object Order(int id) => new
    {
        id, petId = 1, quantity = 1, shipDate = "2026-10-30T13:00:00.000Z",
        status = "placed", complete = true
    };

    public static async Task<JsonElement> Json(HttpResponseMessage response) =>
        await response.Content.ReadFromJsonAsync<JsonElement>();

    public static async Task CreatePet(int id) =>
        await Client.PostAsJsonAsync("pet", Pet(id));

    public static async Task CreateUser(string username, int id) =>
        await Client.PostAsJsonAsync("user", User(username, id));
}
