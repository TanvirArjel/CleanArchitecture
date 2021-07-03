using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using BlazorWasmApp.Extensions;
using BlazorWasmApp.ViewModels.IdentityModels;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace BlazorWasmApp.Common
{
    [TransientService]
    public class AuthorizationDelegatingHandler : DelegatingHandler
    {
        private readonly HostAuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthorizationDelegatingHandler(ILocalStorageService localStorage, HostAuthenticationStateProvider authenticationStateProvider)
        {
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            LoggedInUserInfo loggedInUserInfo = await _localStorage.GetUserInfoAsync();

            if (loggedInUserInfo != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loggedInUserInfo.AccessToken);
            }

            HttpResponseMessage httpResponseMessage = await base.SendAsync(request, cancellationToken);

            if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                Console.WriteLine("Unahtorized response.");
            }

            return httpResponseMessage;
        }
    }
}
