using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace CleanHr.Blazor.Common;

[TransientService]
internal sealed class AuthorizationDelegatingHandler(ILocalStorageService localStorage) : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage = localStorage;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            // LocalStorageService throws exception in when it is being used in .NET MAUI Blazor.
            string jsonWebToken = await _localStorage.GetItemAsync<string>(LocalStorageKey.Jwt, cancellationToken);

            if (string.IsNullOrWhiteSpace(jsonWebToken) == false)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jsonWebToken);
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine("LocalStorageSerivce throws exception.");
            Console.WriteLine(exception);
        }

        HttpResponseMessage httpResponseMessage = await base.SendAsync(request, cancellationToken);

        return httpResponseMessage;
    }
}
