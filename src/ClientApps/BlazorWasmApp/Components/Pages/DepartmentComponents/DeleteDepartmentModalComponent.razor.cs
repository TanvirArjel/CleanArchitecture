using System;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorWasmApp.Common;
using BlazorWasmApp.Extensions;
using BlazorWasmApp.Services;
using BlazorWasmApp.ViewModels.DepartmentsViewModels;
using Microsoft.AspNetCore.Components;
using TanvirArjel.ArgumentChecker;

namespace BlazorWasmApp.Components.Pages.DepartmentComponents
{
    public partial class DeleteDepartmentModalComponent
    {
        private readonly DepartmentService _departmentService;
        private readonly ExceptionLogger _exceptionLogger;

        public DeleteDepartmentModalComponent(DepartmentService departmentService, ExceptionLogger exceptionLogger)
        {
            _departmentService = departmentService;
            _exceptionLogger = exceptionLogger;
        }

        private string ModalClass { get; set; } = string.Empty;

        private bool ShowBackdrop { get; set; }

        private DepartmentDetailsViewModel DepartmentDetailsModel { get; set; }

        private CustomValidator CustomValidator { get; set; }

        [Parameter]
        public EventCallback DepartmentDeleted { get; set; }

        protected override void OnInitialized()
        {
            DepartmentDetailsModel = new DepartmentDetailsViewModel();
        }

        public async Task ShowAsync(int departmentId)
        {
            DepartmentDetailsModel = await _departmentService.GetByIdAsync(departmentId);

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

        private async Task HandleValidSubmit(int departmentId)
        {
            try
            {
                departmentId.ThrowIfZeroOrNegative(nameof(departmentId));

                HttpResponseMessage httpResponseMessage = await _departmentService.DeleteAsync(departmentId);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    Close();
                    await DepartmentDeleted.InvokeAsync();
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
