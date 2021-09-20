using System;
using System.Threading.Tasks;
using BlazorApps.Shared.Models.EmployeeModels;
using BlazorApps.Shared.Services;

namespace BlazorWasmApp.Components.EmployeeComponents
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

        private EmployeeDetailsModel EmployeeDetailsModel { get; set; }

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
