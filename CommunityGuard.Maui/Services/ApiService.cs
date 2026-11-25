using System.Net.Http.Headers;
using System.Net.Http.Json;
using CommunityGuard.Shared.Common;
using CommunityGuard.Shared.Dtos;

namespace CommunityGuard.Maui.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LoginResponse?> LoginAsync(string email, string password)
    {
        var request = new LoginRequest(email, password);
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"API Error ({(int)response.StatusCode}): {errorContent}");
        }

        return await response.Content.ReadFromJsonAsync<LoginResponse>();
    }

    public async Task<DashboardStatsDto?> GetDashboardStatsAsync()
    {
        var client = await GetAuthenticatedClientAsync();
        return await client.GetFromJsonAsync<DashboardStatsDto>("api/dashboard");
    }

    public async Task<PagedResult<IncidentDto>?> GetIncidentsAsync(int page, int pageSize)
    {
        var client = await GetAuthenticatedClientAsync();
        return await client.GetFromJsonAsync<PagedResult<IncidentDto>>($"api/incidents?page={page}&pageSize={pageSize}");
    }

    public async Task CreateIncidentAsync(CreateIncidentRequest request)
    {
        var client = await GetAuthenticatedClientAsync();
        var response = await client.PostAsJsonAsync("api/incidents", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<IEnumerable<ResourceDto>?> GetResourcesAsync()
    {
        var client = await GetAuthenticatedClientAsync();
        return await client.GetFromJsonAsync<IEnumerable<ResourceDto>>("api/resources");
    }

    // Helper method to attach the JWT token to the request.
    private async Task<HttpClient> GetAuthenticatedClientAsync()
    {
        var token = await SecureStorage.Default.GetAsync("jwt_token");
        if (string.IsNullOrEmpty(token))
            throw new Exception("User is not logged in.");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return _httpClient;
    }

    // Add this method inside the ApiService class:
    public async Task AllocateResourceAsync(Guid resourceId, AllocateResourceRequest request)
    {
        var client = await GetAuthenticatedClientAsync();

        // Calls the endpoint we built in Phase 5
        var response = await client.PostAsJsonAsync($"api/resources/{resourceId}/allocate", request);

        // If the API returns 400 (e.g., Double-booking), this will throw an exception.
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to allocate: {error}");
        }
    }

    public async Task<IncidentDetailDto?> GetIncidentByIdAsync(Guid id)
    {
        var client = await GetAuthenticatedClientAsync();
        return await client.GetFromJsonAsync<IncidentDetailDto>($"api/incidents/{id}");
    }

    public async Task UpdateIncidentStatusAsync(Guid id, UpdateIncidentStatusRequest request)
    {
        var client = await GetAuthenticatedClientAsync();
        // Notice we use PatchAsJsonAsync, matching our backend endpoint.
        var response = await client.PatchAsJsonAsync($"api/incidents/{id}/status", request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception(error); // Will trigger the DomainException middleware error
        }
    }
}