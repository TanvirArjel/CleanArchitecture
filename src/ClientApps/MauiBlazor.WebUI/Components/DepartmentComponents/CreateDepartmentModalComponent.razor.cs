using System;
using System.Net.Http;
using System.Threading.Tasks;
using MauiBlazor.Shared.Common;
using MauiBlazor.Shared.Extensions;
using MauiBlazor.Shared.Models.DepartmentModels;
using MauiBlazor.Shared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using TanvirArjel.Blazor;
using TanvirArjel.Blazor.Components;

namespace MauiBlazor.WebUI.Components.DepartmentComponents
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

        private CustomValidationMessages CustomValidationMessages { get; set; }

        private EditContext FormEditContext { get; set; }

        private CreateDepartmentModel CreateDepartmentModel { get; set; } = new CreateDepartmentModel();

        [Parameter]
        public EventCallback DepartmentCreated { get; set; }

        protected override void OnInitialized()
        {
            FormEditContext = new EditContext(CreateDepartmentModel);
            FormEditContext.AddBootstrapValidationClassProvider();
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
            CreateDepartmentModel = new CreateDepartmentModel();
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
                    await DepartmentCreated.InvokeAsync();
                    return;
                }

                await CustomValidationMessages.AddAndDisplayAsync(httpResponseMessage);

            }
            catch (Exception exception)
            {
                CustomValidationMessages.AddAndDisplay(AppErrorMessage.ClientErrorMessage);
                await _exceptionLogger.LogAsync(exception);
            }
        }
    }
}
