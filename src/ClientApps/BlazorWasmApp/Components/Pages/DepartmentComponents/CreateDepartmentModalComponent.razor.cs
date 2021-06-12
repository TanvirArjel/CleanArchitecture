using System;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorWasmApp.Common;
using BlazorWasmApp.Extensions;
using BlazorWasmApp.Services;
using BlazorWasmApp.ViewModels.DepartmentsViewModels;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorWasmApp.Components.Pages.DepartmentComponents
{
    public partial class CreateDepartmentModalComponent
    {
        private readonly DepartmentService _departmentService;
        private readonly ExceptionLogger _exceptionLogger;

        public CreateDepartmentModalComponent(DepartmentService departmentService, ExceptionLogger exceptionLogger)
        {
            _departmentService = departmentService;
            _exceptionLogger = exceptionLogger;
        }

        private string ModalClass { get; set; } = string.Empty;

        private bool ShowBackdrop { get; set; }

        private CustomValidator CustomValidator { get; set; }

        private EditContext FormEditContext { get; set; }

        private CreateDepartmentViewModel CreateDepartmentModel { get; set; } = new CreateDepartmentViewModel();

        protected override void OnInitialized()
        {
            FormEditContext = new EditContext(CreateDepartmentModel);
        }

        public void Show()
        {
            FormEditContext = new EditContext(CreateDepartmentModel);

            ModalClass = "show d-block";
            ShowBackdrop = true;
            StateHasChanged();
        }

        private void Close()
        {
            CreateDepartmentModel = new CreateDepartmentViewModel();
            ModalClass = string.Empty;
            ShowBackdrop = false;
            StateHasChanged();
        }

        public async Task HandleValidSubmit()
        {
            try
            {
                HttpResponseMessage httpResponseMessage = await _departmentService.CreateAsync(CreateDepartmentModel);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    Close();
                    return;
                }

                await CustomValidator.AddErrorsAndDisplayAsync(httpResponseMessage);

            }
            catch (Exception exception)
            {
                CustomValidator.AddErrorAndDisplay(string.Empty, AppErrorMessage.ServerErrorMessage);
                await _exceptionLogger.LogAsync(exception);
            }
        }
    }
}
