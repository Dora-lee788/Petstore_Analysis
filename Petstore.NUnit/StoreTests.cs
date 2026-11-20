using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using PetstoreTests;
using NUnit.Framework;

namespace PetstoreTests;

public class StoreTests
{
    [Test]
    public async Task GetInventory_ShouldReturnInventory()
    {
        var response = await Api.Client.GetAsync("store/inventory");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var inventory = await Api.Json(response);
        Assert.That(inventory.ValueKind, Is.EqualTo(JsonValueKind.Object));
    }

    [Test]
    public async Task CreateOrder_ShouldCreateOrder()
    {
        int id = Api.Id;
        var response = await Api.Client.PostAsJsonAsync("store/order", Api.Order(id));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var order = await Api.Json(response);
        Assert.That(order.GetProperty("id").GetInt32(), Is.EqualTo(id));
    }

    [Test]
    public async Task GetOrder_ShouldReturnOrder()
    {
        int id = Api.Id;
        await Api.Client.PostAsJsonAsync("store/order", Api.Order(id));
        var response = await Api.Client.GetAsync($"store/order/{id}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var order = await Api.Json(response);
        Assert.That(order.GetProperty("status").GetString(), Is.EqualTo("placed"));
    }

    [Test]
    public async Task DeleteOrder_ShouldDeleteOrder()
    {
        int id = Api.Id;
        await Api.Client.PostAsJsonAsync("store/order", Api.Order(id));
        var response = await Api.Client.DeleteAsync($"store/order/{id}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var check = await Api.Client.GetAsync($"store/order/{id}");
        Assert.That(check.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task GetNonexistentOrder_ShouldReturnError()
    {
        var response = await Api.Client.GetAsync($"store/order/{Api.Id}");
        Assert.That(response.StatusCode, Is.AnyOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound));
    }

    [Test]
    public async Task CreateOrderWithInvalidData_ShouldReturnError()
    {
        var response = await Api.Client.PostAsJsonAsync("store/order", new { id = "invalid", petId = "invalid", quantity = -1 });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GetOrderWithInvalidId_ShouldReturnError()
    {
        var response = await Api.Client.GetAsync("store/order/invalid");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task DeleteNonexistentOrder_ShouldReturnError()
    {
        var response = await Api.Client.DeleteAsync($"store/order/{Api.Id}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

}