using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorWasmApp.Common
{
    public class CustomValidator : ComponentBase
    {
        private ValidationMessageStore _messageStore;

        [CascadingParameter]
        public EditContext CurrentEditContext { get; set; }

        public void DisplayErrors()
        {
            CurrentEditContext.NotifyValidationStateChanged();
        }

        public void AddErrorAndDisplay(string key, string errorMessage)
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>()
            {
                { key, new List<string> { errorMessage } }
            };

            AddErrorsAndDiplay(errors);
        }

        public void AddError(string key, string errorMessage)
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>()
            {
                { key, new List<string> { errorMessage } }
            };

            AddErrors(errors);
        }

        public void AddErrorsAndDiplay(IDictionary<string, List<string>> errors)
        {
            AddErrors(errors);
            CurrentEditContext.NotifyValidationStateChanged();
        }

        public void AddErrors(IDictionary<string, List<string>> errors)
        {
            if (errors == null || errors.Count == 0)
            {
                return;
            }

            PropertyInfo[] propertyInfos = CurrentEditContext.Model.GetType().GetProperties();

            foreach (KeyValuePair<string, List<string>> err in errors)
            {
                if (propertyInfos.Any())
                {
                    bool isExistent = propertyInfos.Any(pi => pi.Name.Equals(err.Key, StringComparison.OrdinalIgnoreCase));

                    if (isExistent)
                    {
                        _messageStore.Add(CurrentEditContext.Field(err.Key), err.Value);
                    }
                    else
                    {
                        _messageStore.Add(CurrentEditContext.Field(string.Empty), err.Value);
                    }
                }
                else
                {
                    _messageStore.Add(CurrentEditContext.Field(string.Empty), err.Value);
                }
            }
        }

        public void ClearErrors()
        {
            _messageStore.Clear();
            CurrentEditContext.NotifyValidationStateChanged();
        }

        protected override void OnInitialized()
        {
            if (CurrentEditContext == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(CustomValidator)} requires a cascading " +
                    $"parameter of type {nameof(EditContext)}. " +
                    $"For example, you can use {nameof(CustomValidator)} " +
                    $"inside an {nameof(EditForm)}.");
            }

            _messageStore = new ValidationMessageStore(CurrentEditContext);

            CurrentEditContext.OnValidationRequested += (s, e) =>
                _messageStore.Clear();
            CurrentEditContext.OnFieldChanged += (s, e) =>
                _messageStore.Clear(e.FieldIdentifier);
        }
    }
}
