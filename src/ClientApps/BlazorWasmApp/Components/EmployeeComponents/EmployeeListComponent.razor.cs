using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientApps.Shared.Models.EmployeeViewModels;
using ClientApps.Shared.Services;

namespace BlazorWasmApp.Components.EmployeeComponents
{
    public partial class EmployeeListComponent
    {
        private readonly EmployeeService _employeeService;

        public EmployeeListComponent(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        private List<EmployeeDetailsViewModel> Employees { get; set; }

        private CreateEmployeeModalComponent CreateModal { get; set; }

        private UpdateEmployeeModalComponent UpdateModal { get; set; }

        private EmployeeDetailsModalComponent DetailsModal { get; set; }

        private DeleteEmployeeModalComponent DeleteModal { get; set; }

        private string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Employees = await _employeeService.GetListAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                ErrorMessage = "There is some error.";
            }
        }

        private async Task OpenCreateModalAsync()
        {
            await CreateModal.OpenAsync();
        }

        private async Task OpenUpdateModalAsync(Guid employeeId)
        {
            await UpdateModal.OpenAsync(employeeId);
        }

        private async Task OpenDetailsModalAsync(Guid employeeId)
        {
            await DetailsModal.OpenAsync(employeeId);
        }

        private async Task OpenDeleteModalAsync(Guid employeeId)
        {
            await DeleteModal.OpenAsync(employeeId);
        }

        private async void CreatedEmployeeEventHandler()
        {
            Employees = await _employeeService.GetListAsync();
            StateHasChanged();
        }

        private async void EmployeeUpdatedEventHandler()
        {
            Employees = await _employeeService.GetListAsync();
            StateHasChanged();
        }

        private async void EmployeeDeletedEventHandler()
        {
            Employees = await _employeeService.GetListAsync();
            StateHasChanged();
        }
    }
}
