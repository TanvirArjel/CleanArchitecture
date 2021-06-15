﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorWasmApp.Common;
using BlazorWasmApp.Extensions;
using BlazorWasmApp.Services;
using BlazorWasmApp.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Components;

namespace BlazorWasmApp.Components.Pages.EmployeeComponents
{
    public partial class DeleteEmployeeModalComponent
    {
        private readonly EmployeeService _employeeService;
        private readonly ExceptionLogger _exceptionLogger;

        public DeleteEmployeeModalComponent(EmployeeService employeeService, ExceptionLogger exceptionLogger)
        {
            _employeeService = employeeService;
            _exceptionLogger = exceptionLogger;
        }

        private string ModalClass { get; set; } = string.Empty;

        private bool ShowBackdrop { get; set; }

        private CustomValidator CustomValidator { get; set; }

        private EmployeeDetailsViewModel EmployeeDetailsModel { get; set; } = new EmployeeDetailsViewModel();

        [Parameter]
        public EventCallback EmployeeDeleted { get; set; }

        public async Task OpenAsync(int employeeId)
        {
            EmployeeDetailsModel = await _employeeService.GetDetailsByIdAsync(employeeId);
            ModalClass = "show d-block";
            ShowBackdrop = true;
            StateHasChanged();
        }

        private void Close()
        {
            ModalClass = string.Empty;
            ShowBackdrop = false;
            StateHasChanged();
        }

        private async Task HandleValidSubmit()
        {
            try
            {
                HttpResponseMessage httpResponseMessage = await _employeeService.DeleteAsync(EmployeeDetailsModel.Id);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    await EmployeeDeleted.InvokeAsync();
                    Close();
                    return;
                }

                await CustomValidator.AddErrorsAndDisplayAsync(httpResponseMessage);

            }
            catch (Exception exception)
            {
                CustomValidator.AddErrorAndDisplay(string.Empty, AppErrorMessage.ClientErrorMessage);
                await _exceptionLogger.LogAsync(exception);
            }

        }
    }
}