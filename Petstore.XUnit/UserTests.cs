using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using PetstoreTests;
using Xunit;

namespace PetstoreTests;

public class UserTests
{
    [Fact]
    public async Task CreateUser_ShouldCreateUser()
    {
        string username = Api.Username; int id = Api.Id;
        var response = await Api.Client.PostAsJsonAsync("user", Api.User(username, id));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetUser_ShouldReturnUser()
    {
        string username = Api.Username; int id = Api.Id;
        await Api.CreateUser(username, id);
        var response = await Api.Client.GetAsync($"user/{username}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await Api.Json(response);
        Assert.Equal(username, user.GetProperty("username").GetString());
    }

    [Fact]
    public async Task UpdateUser_ShouldUpdateUser()
    {
        string username = Api.Username; int id = Api.Id;
        await Api.CreateUser(username, id);
        var data = new { id, username, firstName = "Updated", lastName = "User", email = "updated@example.com", password = "654321", phone = "9876543210", userStatus = 1 };
        var response = await Api.Client.PutAsJsonAsync($"user/{username}", data);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await Api.Json(await Api.Client.GetAsync($"user/{username}"));
        Assert.Equal("Updated", result.GetProperty("firstName").GetString());
    }

    [Fact]
    public async Task DeleteUser_ShouldDeleteUser()
    {
        string username = Api.Username; int id = Api.Id;
        await Api.CreateUser(username, id);
        var response = await Api.Client.DeleteAsync($"user/{username}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var check = await Api.Client.GetAsync($"user/{username}");
        Assert.Equal(HttpStatusCode.NotFound, check.StatusCode);
    }

    [Fact]
    public async Task GetNonexistentUser_ShouldReturn404()
    {
        var response = await Api.Client.GetAsync($"user/{Api.Username}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateUserWithInvalidData_ShouldReturnError()
    {
        var response = await Api.Client.PostAsJsonAsync("user", new { id = "invalid", username = 123 });
        Assert.Contains(response.StatusCode, new[] { HttpStatusCode.BadRequest, HttpStatusCode.MethodNotAllowed });
    }

    [Fact]
    public async Task UpdateNonexistentUser_ShouldReturn404()
    {
        var response = await Api.Client.PutAsJsonAsync($"user/{Api.Username}", Api.User(Api.Username, Api.Id));
        Assert.Contains(response.StatusCode, new[] { HttpStatusCode.NotFound, HttpStatusCode.BadRequest });
    }

    [Fact]
    public async Task DeleteNonexistentUser_ShouldReturn404()
    {
        var response = await Api.Client.DeleteAsync($"user/{Api.Username}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateUsersWithArray_ShouldCreateUser()
    {
        string username = Api.Username; int id = Api.Id;
        var users = new[] { Api.User(username, id) };
        var response = await Api.Client.PostAsJsonAsync("user/createWithArray", users);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var check = await Api.Client.GetAsync($"user/{username}");
        Assert.Equal(HttpStatusCode.OK, check.StatusCode);
    }

    [Fact]
    public async Task CreateUsersWithInvalidArray_ShouldReturnError()
    {
        var response = await Api.Client.PostAsJsonAsync("user/createWithArray", new { username = 123 });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateUsersWithList_ShouldCreateUser()
    {
        string username = Api.Username; int id = Api.Id;
        var users = new[] { Api.User(username, id) };
        var response = await Api.Client.PostAsJsonAsync("user/createWithList", users);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var check = await Api.Client.GetAsync($"user/{username}");
        Assert.Equal(HttpStatusCode.OK, check.StatusCode);
    }

    [Fact]
    public async Task CreateUsersWithInvalidList_ShouldReturnError()
    {
        var response = await Api.Client.PostAsJsonAsync("user/createWithList", new { username = 123 });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task LoginUser_ShouldReturn200()
    {
        string username = Api.Username; int id = Api.Id;
        await Api.CreateUser(username, id);
        var response = await Api.Client.GetAsync($"user/login?username={username}&password=123456");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task LoginWithInvalidCredentials_ShouldReturn400()
    {
        var response = await Api.Client.GetAsync("user/login?username=missing_user&password=wrong");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task LogoutUser_ShouldReturn200()
    {
        var response = await Api.Client.GetAsync("user/logout");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task LogoutWithInvalidMethod_ShouldReturn405()
    {
        var response = await Api.Client.PostAsync("user/logout", null);
        Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
    }

}