using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using PetstoreTests;
using NUnit.Framework;

namespace PetstoreTests;

public class PetTests
{
    [Test]
    public async Task CreatePet_ShouldReturnCreatedPet()
    {
        int id = Api.Id;
        var response = await Api.Client.PostAsJsonAsync("pet", Api.Pet(id));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var pet = await Api.Json(response);
        Assert.That(pet.GetProperty("id").GetInt32(), Is.EqualTo(id));
    }

    [Test]
    public async Task GetPet_ShouldReturnExistingPet()
    {
        int id = Api.Id;
        await Api.CreatePet(id);
        var response = await Api.Client.GetAsync($"pet/{id}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var pet = await Api.Json(response);
        Assert.That(pet.GetProperty("name").GetString(), Is.EqualTo("TestPet"));
    }

    [Test]
    public async Task UpdatePet_ShouldUpdateNameAndStatus()
    {
        int id = Api.Id;
        await Api.CreatePet(id);
        var response = await Api.Client.PutAsJsonAsync("pet", Api.Pet(id, "UpdatedPet", "sold"));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var pet = await Api.Json(await Api.Client.GetAsync($"pet/{id}"));
        Assert.That(pet.GetProperty("name").GetString(), Is.EqualTo("UpdatedPet"));
        Assert.That(pet.GetProperty("status").GetString(), Is.EqualTo("sold"));
    }

    [Test]
    public async Task DeletePet_ShouldDeletePet()
    {
        int id = Api.Id;
        await Api.CreatePet(id);
        var response = await Api.Client.DeleteAsync($"pet/{id}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var check = await Api.Client.GetAsync($"pet/{id}");
        Assert.That(check.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task GetNonexistentPet_ShouldReturn404()
    {
        var response = await Api.Client.GetAsync($"pet/{Api.Id}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task CreatePetWithInvalidData_ShouldReturnError()
    {
        var response = await Api.Client.PostAsJsonAsync("pet", new { id = "invalid", name = 123 });
        Assert.That(response.StatusCode, Is.AnyOf(HttpStatusCode.BadRequest, HttpStatusCode.MethodNotAllowed));
    }

    [Test]
    public async Task UpdatePetWithInvalidData_ShouldReturnError()
    {
        var response = await Api.Client.PutAsJsonAsync("pet", new { id = "invalid", name = 123, photoUrls = Array.Empty<string>() });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task DeleteNonexistentPet_ShouldReturn404()
    {
        var response = await Api.Client.DeleteAsync($"pet/{Api.Id}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task FindPetsByStatus_ShouldReturnMatchingPets()
    {
        var response = await Api.Client.GetAsync("pet/findByStatus?status=available");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var pets = await Api.Json(response);
        Assert.That(pets.ValueKind, Is.EqualTo(JsonValueKind.Array));
    }

    [Test]
    public async Task FindPetsByInvalidStatus_ShouldReturnError()
    {
        var response = await Api.Client.GetAsync("pet/findByStatus?status=invalid_status");
        Assert.That(response.StatusCode, Is.AnyOf(HttpStatusCode.BadRequest, HttpStatusCode.OK));
    }

    [Test]
    public async Task FindPetsByTags_ShouldReturnList()
    {
        var response = await Api.Client.GetAsync("pet/findByTags?tags=1");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var pets = await Api.Json(response);
        Assert.That(pets.ValueKind, Is.EqualTo(JsonValueKind.Array));
    }

    [Test]
    public async Task FindPetsByNonexistentTag_ShouldReturnEmptyList()
    {
        var response = await Api.Client.GetAsync("pet/findByTags?tags=999999999");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var pets = await Api.Json(response);
        Assert.That(pets.GetArrayLength(), Is.EqualTo(0));
    }

    [Test]
    public async Task UpdatePetWithForm_ShouldUpdatePet()
    {
        int id = Api.Id;
        await Api.CreatePet(id);
        var response = await Api.Client.PostAsync($"pet/{id}", new FormUrlEncodedContent(new Dictionary<string, string> { ["name"] = "FormPetUpdated", ["status"] = "sold" }));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var pet = await Api.Json(await Api.Client.GetAsync($"pet/{id}"));
        Assert.That(pet.GetProperty("name").GetString(), Is.EqualTo("FormPetUpdated"));
    }

    [Test]
    public async Task UpdateNonexistentPetWithForm_ShouldReturnError()
    {
        var response = await Api.Client.PostAsync("pet/999999999", new FormUrlEncodedContent(new Dictionary<string, string> { ["name"] = "MissingPet", ["status"] = "sold" }));
        Assert.That(response.StatusCode, Is.AnyOf(HttpStatusCode.NotFound, HttpStatusCode.MethodNotAllowed));
    }

}