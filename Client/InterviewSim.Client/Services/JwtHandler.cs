using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace InterviewSim.Client.Services;

public class JwtHandler : DelegatingHandler
{
    private readonly ILocalStorageService _storage;
    private readonly LoadingState _loading;

    public JwtHandler(ILocalStorageService storage, LoadingState loading)
    {
        _storage = storage;
        _loading = loading;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _storage.GetItemAsync<string>("token");
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        try
        {
            _loading.IsLoading = true;
            return await base.SendAsync(request, cancellationToken);
        }
        finally
        {
            _loading.IsLoading = false;
        }
    }
}
