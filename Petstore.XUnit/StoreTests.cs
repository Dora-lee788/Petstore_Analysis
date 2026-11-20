using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using PetstoreTests;
using Xunit;

namespace PetstoreTests;

public class StoreTests
{
    [Fact]
    public async Task GetInventory_ShouldReturnInventory()
    {
        var response = await Api.Client.GetAsync("store/inventory");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var inventory = await Api.Json(response);
        Assert.True(inventory.ValueKind == JsonValueKind.Object);
    }

    [Fact]
    public async Task CreateOrder_ShouldCreateOrder()
    {
        int id = Api.Id;
        var response = await Api.Client.PostAsJsonAsync("store/order", Api.Order(id));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var order = await Api.Json(response);
        Assert.Equal(id, order.GetProperty("id").GetInt32());
    }

    [Fact]
    public async Task GetOrder_ShouldReturnOrder()
    {
        int id = Api.Id;
        await Api.Client.PostAsJsonAsync("store/order", Api.Order(id));
        var response = await Api.Client.GetAsync($"store/order/{id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var order = await Api.Json(response);
        Assert.Equal("placed", order.GetProperty("status").GetString());
    }

    [Fact]
    public async Task DeleteOrder_ShouldDeleteOrder()
    {
        int id = Api.Id;
        await Api.Client.PostAsJsonAsync("store/order", Api.Order(id));
        var response = await Api.Client.DeleteAsync($"store/order/{id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var check = await Api.Client.GetAsync($"store/order/{id}");
        Assert.Equal(HttpStatusCode.NotFound, check.StatusCode);
    }

    [Fact]
    public async Task GetNonexistentOrder_ShouldReturnError()
    {
        var response = await Api.Client.GetAsync($"store/order/{Api.Id}");
        Assert.Contains(response.StatusCode, new[] { HttpStatusCode.BadRequest, HttpStatusCode.NotFound });
    }

    [Fact]
    public async Task CreateOrderWithInvalidData_ShouldReturnError()
    {
        var response = await Api.Client.PostAsJsonAsync("store/order", new { id = "invalid", petId = "invalid", quantity = -1 });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetOrderWithInvalidId_ShouldReturnError()
    {
        var response = await Api.Client.GetAsync("store/order/invalid");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteNonexistentOrder_ShouldReturnError()
    {
        var response = await Api.Client.DeleteAsync($"store/order/{Api.Id}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

}