using System;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorWasmApp.Common;
using BlazorWasmApp.Services;
using BlazorWasmApp.ViewModels.DepartmentsViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using TanvirArjel.Blazor.Components;

namespace BlazorWasmApp.Components.DepartmentComponents
{
    public partial class UpdateDepartmentModalComponent
    {
        private readonly DepartmentService _departmentService;
        private readonly ExceptionLogger _exceptionLogger;

        public UpdateDepartmentModalComponent(DepartmentService departmentService, ExceptionLogger exceptionLogger)
        {
            _departmentService = departmentService;
            _exceptionLogger = exceptionLogger;
        }

        private string ModalClass { get; set; } = string.Empty;

        private bool ShowBackdrop { get; set; }

        private CustomValidationMessages CustomValidationMessages { get; set; }

        private EditContext FormEditContext { get; set; }

        private UpdateDepartmentViewModel UpdateDepartmentModel { get; set; } = new UpdateDepartmentViewModel();

        private string ErrorMessage { get; set; }

        [Parameter]
        public EventCallback DepartmentUpdated { get; set; }

        public async Task ShowAsync(Guid departmentId)
        {
            DepartmentDetailsViewModel departmentDetailsViewModel = await _departmentService.GetByIdAsync(departmentId);

            if (departmentDetailsViewModel == null)
            {
                ErrorMessage = "Employee with provided id does not exists!";
            }

            UpdateDepartmentModel = new UpdateDepartmentViewModel()
            {
                Id = departmentDetailsViewModel.Id,
                Name = departmentDetailsViewModel.Name,
                Description = departmentDetailsViewModel.Description,
            };

            FormEditContext = new EditContext(UpdateDepartmentModel);

            ModalClass = "show d-block";
            ShowBackdrop = true;
            StateHasChanged();
        }

        private void Close()
        {
            UpdateDepartmentModel = new UpdateDepartmentViewModel();
            ModalClass = string.Empty;
            ShowBackdrop = false;
            StateHasChanged();
        }

        protected override void OnInitialized()
        {
            FormEditContext = new EditContext(UpdateDepartmentModel);
        }

        private async Task HandleValidSubmit()
        {
            try
            {
                HttpResponseMessage httpResponseMessage = await _departmentService.UpdateAsync(UpdateDepartmentModel);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    Close();
                    await DepartmentUpdated.InvokeAsync();
                    return;
                }

                await CustomValidationMessages.AddAndDisplayAsync(httpResponseMessage);
            }
            catch (Exception exception)
            {
                await _exceptionLogger.LogAsync(exception);
                CustomValidationMessages.AddAndDisplay(string.Empty, AppErrorMessage.ClientErrorMessage);
            }
        }
    }
}
