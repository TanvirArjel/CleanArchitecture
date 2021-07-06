using System;
using System.Threading.Tasks;
using MauiBlazor.Shared.Models.EmployeeViewModels;
using MauiBlazor.Shared.Services;

namespace MauiBlazor.WebUI.Components.EmployeeComponents
{
    public partial class EmployeeDetailsModalComponent
    {
        private readonly EmployeeService _employeeService;

        public EmployeeDetailsModalComponent(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        private string ModalClass { get; set; } = string.Empty;

        private bool ShowBackdrop { get; set; }

        private EmployeeDetailsViewModel EmployeeDetailsModel { get; set; }

        public async Task OpenAsync(Guid employeeId)
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
    }
}
