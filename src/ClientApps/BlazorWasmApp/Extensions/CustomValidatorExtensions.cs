using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BlazorWasmApp.Common;

namespace BlazorWasmApp.Extensions
{
    public static class CustomValidatorExtensions
    {
        public static async Task AddErrorsAndDisplayAsync(this CustomValidator customValidator, HttpResponseMessage httpResponseMessage)
        {
            await AddErrorsAsync(customValidator, httpResponseMessage);
            customValidator.DisplayErrors();
        }
        public static async Task AddErrorsAsync(this CustomValidator customValidator, HttpResponseMessage httpResponseMessage)
        {
            if (customValidator == null)
            {
                throw new ArgumentNullException(nameof(customValidator));
            }

            if (httpResponseMessage == null)
            {
                throw new ArgumentNullException(nameof(httpResponseMessage));
            }

            if (httpResponseMessage.IsSuccessStatusCode == false)
            {
                Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

                if ((int)httpResponseMessage.StatusCode == 400)
                {
                    JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    string responseString = await httpResponseMessage.Content.ReadAsStringAsync();
                    Dictionary<string, List<string>> modelErrors = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(responseString, jsonSerializerOptions);
                    if (modelErrors.Any())
                    {
                        foreach (KeyValuePair<string, List<string>> error in modelErrors)
                        {
                            string fieldName = error.Key;
                            List<string> errorMessages = error.Value;
                            errors.Add(fieldName, errorMessages);
                        }
                    }
                    else
                    {
                        errors.Add(string.Empty, new List<string>() { "Invalid request. One or more validations failed." });
                    }
                }
                else if ((int)httpResponseMessage.StatusCode == 401)
                {
                    errors.Add(string.Empty, new List<string>() { AppErrorMessage.UnAuthorizedErrorMessage });
                }
                else
                {
                    errors.Add(string.Empty, new List<string> { AppErrorMessage.ServerErrorMessage });
                }

                customValidator.AddErrors(errors);
            }
        }
    }
}
