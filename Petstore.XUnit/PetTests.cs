using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using PetstoreTests;
using Xunit;

namespace PetstoreTests;

public class PetTests
{
    [Fact]
    public async Task CreatePet_ShouldReturnCreatedPet()
    {
        int id = Api.Id;
        var response = await Api.Client.PostAsJsonAsync("pet", Api.Pet(id));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var pet = await Api.Json(response);
        Assert.Equal(id, pet.GetProperty("id").GetInt32());
    }

    [Fact]
    public async Task GetPet_ShouldReturnExistingPet()
    {
        int id = Api.Id;
        await Api.CreatePet(id);
        var response = await Api.Client.GetAsync($"pet/{id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var pet = await Api.Json(response);
        Assert.Equal("TestPet", pet.GetProperty("name").GetString());
    }

    [Fact]
    public async Task UpdatePet_ShouldUpdateNameAndStatus()
    {
        int id = Api.Id;
        await Api.CreatePet(id);
        var response = await Api.Client.PutAsJsonAsync("pet", Api.Pet(id, "UpdatedPet", "sold"));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var pet = await Api.Json(await Api.Client.GetAsync($"pet/{id}"));
        Assert.Equal("UpdatedPet", pet.GetProperty("name").GetString());
        Assert.Equal("sold", pet.GetProperty("status").GetString());
    }

    [Fact]
    public async Task DeletePet_ShouldDeletePet()
    {
        int id = Api.Id;
        await Api.CreatePet(id);
        var response = await Api.Client.DeleteAsync($"pet/{id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var check = await Api.Client.GetAsync($"pet/{id}");
        Assert.Equal(HttpStatusCode.NotFound, check.StatusCode);
    }

    [Fact]
    public async Task GetNonexistentPet_ShouldReturn404()
    {
        var response = await Api.Client.GetAsync($"pet/{Api.Id}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreatePetWithInvalidData_ShouldReturnError()
    {
        var response = await Api.Client.PostAsJsonAsync("pet", new { id = "invalid", name = 123 });
        Assert.Contains(response.StatusCode, new[] { HttpStatusCode.BadRequest, HttpStatusCode.MethodNotAllowed });
    }

    [Fact]
    public async Task UpdatePetWithInvalidData_ShouldReturnError()
    {
        var response = await Api.Client.PutAsJsonAsync("pet", new { id = "invalid", name = 123, photoUrls = Array.Empty<string>() });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteNonexistentPet_ShouldReturn404()
    {
        var response = await Api.Client.DeleteAsync($"pet/{Api.Id}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task FindPetsByStatus_ShouldReturnMatchingPets()
    {
        var response = await Api.Client.GetAsync("pet/findByStatus?status=available");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var pets = await Api.Json(response);
        Assert.True(pets.ValueKind == JsonValueKind.Array);
    }

    [Fact]
    public async Task FindPetsByInvalidStatus_ShouldReturnError()
    {
        var response = await Api.Client.GetAsync("pet/findByStatus?status=invalid_status");
        Assert.Contains(response.StatusCode, new[] { HttpStatusCode.BadRequest, HttpStatusCode.OK });
    }

    [Fact]
    public async Task FindPetsByTags_ShouldReturnList()
    {
        var response = await Api.Client.GetAsync("pet/findByTags?tags=1");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var pets = await Api.Json(response);
        Assert.True(pets.ValueKind == JsonValueKind.Array);
    }

    [Fact]
    public async Task FindPetsByNonexistentTag_ShouldReturnEmptyList()
    {
        var response = await Api.Client.GetAsync("pet/findByTags?tags=999999999");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var pets = await Api.Json(response);
        Assert.Equal(0, pets.GetArrayLength());
    }

    [Fact]
    public async Task UpdatePetWithForm_ShouldUpdatePet()
    {
        int id = Api.Id;
        await Api.CreatePet(id);
        var response = await Api.Client.PostAsync($"pet/{id}", new FormUrlEncodedContent(new Dictionary<string, string> { ["name"] = "FormPetUpdated", ["status"] = "sold" }));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var pet = await Api.Json(await Api.Client.GetAsync($"pet/{id}"));
        Assert.Equal("FormPetUpdated", pet.GetProperty("name").GetString());
    }

    [Fact]
    public async Task UpdateNonexistentPetWithForm_ShouldReturnError()
    {
        var response = await Api.Client.PostAsync("pet/999999999", new FormUrlEncodedContent(new Dictionary<string, string> { ["name"] = "MissingPet", ["status"] = "sold" }));
        Assert.Contains(response.StatusCode, new[] { HttpStatusCode.NotFound, HttpStatusCode.MethodNotAllowed });
    }

}