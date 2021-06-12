using System;
using System.Collections.Generic;
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

        public void AddErrorsAndDiplay(IDictionary<string, List<string>> errors)
        {
            AddErrors(errors);
            CurrentEditContext.NotifyValidationStateChanged();
        }

        public void AddErrors(IDictionary<string, List<string>> errors)
        {
            if (errors != null)
            {
                foreach (KeyValuePair<string, List<string>> err in errors)
                {
                    _messageStore.Add(CurrentEditContext.Field(err.Key), err.Value);
                }
            }
        }

        public void AddErrorAndDisplay(string key, string errorMessage)
        {
            _messageStore.Add(CurrentEditContext.Field(key), errorMessage);
            CurrentEditContext.NotifyValidationStateChanged();
        }

        public void AddError(string key, string errorMessage)
        {
            _messageStore.Add(CurrentEditContext.Field(key), errorMessage);
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
