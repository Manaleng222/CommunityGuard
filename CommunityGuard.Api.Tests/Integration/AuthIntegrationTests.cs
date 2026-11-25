using CommunityGuard.Shared.Dtos;
using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CommunityGuard.Api.Tests.Integration;

public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient(); // Creates an HTTP client pointing to the fake API.
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_Returns401()
    {
        // Arrange: A fake login request with wrong details.
        var request = new Shared.Dtos.LoginRequest("nobody@test.com", "WrongPassword123!");

        // Act: Send it to the login endpoint.
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        // Assert: Should be rejected.
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_Returns401()
    {
        // Act: Try to access a secure endpoint without logging in.
        var response = await _client.GetAsync("/api/incidents");

        // Assert: The API should block it.
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}