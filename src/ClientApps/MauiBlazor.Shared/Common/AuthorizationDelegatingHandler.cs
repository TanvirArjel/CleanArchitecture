using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using MauiBlazor.Shared.Models.IdentityModels;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace MauiBlazor.Shared.Common
{
    [TransientService]
    public class AuthorizationDelegatingHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public AuthorizationDelegatingHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            try
            {
                // LocalStorageService throws exception in when it is being used in .NET MAUI Blazor as it is registered as scoped service
                // but it needs to be registered as Transient but no such option available right now.
                LoggedInUserInfo loggedInUserInfo = await _localStorage.GetItemAsync<LoggedInUserInfo>(LocalStorageKey.Jwt);

                if (loggedInUserInfo != null)
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loggedInUserInfo.AccessToken);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("LocalStorageSerivce throws exception.");
            }

            HttpResponseMessage httpResponseMessage = await base.SendAsync(request, cancellationToken);

            return httpResponseMessage;
        }
    }
}
