using System;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorWasmApp.Common;
using BlazorWasmApp.Services;
using BlazorWasmApp.ViewModels.DepartmentsViewModels;
using Microsoft.AspNetCore.Components;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.Blazor;
using TanvirArjel.Blazor.Components;

namespace BlazorWasmApp.Components.DepartmentComponents
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

        private CustomValidationMessages CustomValidationMessages { get; set; }

        [Parameter]
        public EventCallback DepartmentDeleted { get; set; }

        protected override void OnInitialized()
        {
            DepartmentDetailsModel = new DepartmentDetailsViewModel();
        }

        public async Task ShowAsync(Guid departmentId)
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

        private async Task HandleValidSubmit(Guid departmentId)
        {
            try
            {
                departmentId.ThrowIfEmpty(nameof(departmentId));

                HttpResponseMessage httpResponseMessage = await _departmentService.DeleteAsync(departmentId);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    Close();
                    await DepartmentDeleted.InvokeAsync();
                    return;
                }

                await CustomValidationMessages.AddAndDisplayAsync(httpResponseMessage);

            }
            catch (Exception exception)
            {
                CustomValidationMessages.AddAndDisplay(string.Empty, AppErrorMessage.ClientErrorMessage);
                await _exceptionLogger.LogAsync(exception);
            }

        }
    }
}
