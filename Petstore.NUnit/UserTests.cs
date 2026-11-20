using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using PetstoreTests;
using NUnit.Framework;

namespace PetstoreTests;

public class UserTests
{
    [Test]
    public async Task CreateUser_ShouldCreateUser()
    {
        string username = Api.Username; int id = Api.Id;
        var response = await Api.Client.PostAsJsonAsync("user", Api.User(username, id));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetUser_ShouldReturnUser()
    {
        string username = Api.Username; int id = Api.Id;
        await Api.CreateUser(username, id);
        var response = await Api.Client.GetAsync($"user/{username}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var user = await Api.Json(response);
        Assert.That(user.GetProperty("username").GetString(), Is.EqualTo(username));
    }

    [Test]
    public async Task UpdateUser_ShouldUpdateUser()
    {
        string username = Api.Username; int id = Api.Id;
        await Api.CreateUser(username, id);
        var data = new { id, username, firstName = "Updated", lastName = "User", email = "updated@example.com", password = "654321", phone = "9876543210", userStatus = 1 };
        var response = await Api.Client.PutAsJsonAsync($"user/{username}", data);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var result = await Api.Json(await Api.Client.GetAsync($"user/{username}"));
        Assert.That(result.GetProperty("firstName").GetString(), Is.EqualTo("Updated"));
    }

    [Test]
    public async Task DeleteUser_ShouldDeleteUser()
    {
        string username = Api.Username; int id = Api.Id;
        await Api.CreateUser(username, id);
        var response = await Api.Client.DeleteAsync($"user/{username}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var check = await Api.Client.GetAsync($"user/{username}");
        Assert.That(check.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task GetNonexistentUser_ShouldReturn404()
    {
        var response = await Api.Client.GetAsync($"user/{Api.Username}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task CreateUserWithInvalidData_ShouldReturnError()
    {
        var response = await Api.Client.PostAsJsonAsync("user", new { id = "invalid", username = 123 });
        Assert.That(response.StatusCode, Is.AnyOf(HttpStatusCode.BadRequest, HttpStatusCode.MethodNotAllowed));
    }

    [Test]
    public async Task UpdateNonexistentUser_ShouldReturn404()
    {
        var response = await Api.Client.PutAsJsonAsync($"user/{Api.Username}", Api.User(Api.Username, Api.Id));
        Assert.That(response.StatusCode, Is.AnyOf(HttpStatusCode.NotFound, HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task DeleteNonexistentUser_ShouldReturn404()
    {
        var response = await Api.Client.DeleteAsync($"user/{Api.Username}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task CreateUsersWithArray_ShouldCreateUser()
    {
        string username = Api.Username; int id = Api.Id;
        var users = new[] { Api.User(username, id) };
        var response = await Api.Client.PostAsJsonAsync("user/createWithArray", users);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var check = await Api.Client.GetAsync($"user/{username}");
        Assert.That(check.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task CreateUsersWithInvalidArray_ShouldReturnError()
    {
        var response = await Api.Client.PostAsJsonAsync("user/createWithArray", new { username = 123 });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task CreateUsersWithList_ShouldCreateUser()
    {
        string username = Api.Username; int id = Api.Id;
        var users = new[] { Api.User(username, id) };
        var response = await Api.Client.PostAsJsonAsync("user/createWithList", users);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var check = await Api.Client.GetAsync($"user/{username}");
        Assert.That(check.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task CreateUsersWithInvalidList_ShouldReturnError()
    {
        var response = await Api.Client.PostAsJsonAsync("user/createWithList", new { username = 123 });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task LoginUser_ShouldReturn200()
    {
        string username = Api.Username; int id = Api.Id;
        await Api.CreateUser(username, id);
        var response = await Api.Client.GetAsync($"user/login?username={username}&password=123456");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(await response.Content.ReadAsStringAsync(), Is.Not.Empty);
    }

    [Test]
    public async Task LoginWithInvalidCredentials_ShouldReturn400()
    {
        var response = await Api.Client.GetAsync("user/login?username=missing_user&password=wrong");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task LogoutUser_ShouldReturn200()
    {
        var response = await Api.Client.GetAsync("user/logout");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task LogoutWithInvalidMethod_ShouldReturn405()
    {
        var response = await Api.Client.PostAsync("user/logout", null);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed));
    }

}