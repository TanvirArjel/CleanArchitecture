using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using MauiBlazor.Shared.Extensions;
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

            LoggedInUserInfo loggedInUserInfo = await _localStorage.GetUserInfoAsync();

            if (loggedInUserInfo != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loggedInUserInfo.AccessToken);
            }

            HttpResponseMessage httpResponseMessage = await base.SendAsync(request, cancellationToken);

            return httpResponseMessage;
        }
    }
}
