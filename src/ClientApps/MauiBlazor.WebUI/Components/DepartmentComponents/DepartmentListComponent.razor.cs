using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MauiBlazor.Shared.Common;
using MauiBlazor.Shared.Models.DepartmentModels;
using MauiBlazor.Shared.Services;

namespace MauiBlazor.WebUI.Components.DepartmentComponents
{
    public partial class DepartmentListComponent
    {
        private readonly DepartmentService _departmentService;
        private readonly ExceptionLogger _exceptionLogger;

        public DepartmentListComponent(DepartmentService departmentService, ExceptionLogger exceptionLogger)
        {
            _departmentService = departmentService;
            _exceptionLogger = exceptionLogger;
        }

        private List<DepartmentDetailsModel> Departments { get; set; }

        private string ErrorMessage { get; set; }

        private CreateDepartmentModalComponent CreateModal { get; set; }

        private UpdateDepartmentModalComponent UpdateModal { get; set; }

        private DepartmentDetailsModalComponent DetailsModal { get; set; }

        private DeleteDepartmentModalComponent DeleteModal { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await LoadDepartmentsAsync();
            }
            catch (HttpRequestException httpException)
            {
                Console.WriteLine(httpException);
                ErrorMessage = "The server maybe down or the app does not have CORS access to the requested server.";
            }
            catch (Exception exception)
            {
                await _exceptionLogger.LogAsync(exception);
                ErrorMessage = "There is some problem with the service. Please try again. If the problem persists please contact with system administrator.";
            }
        }

        // EventHandler which will be called whenvever ItemUpdated event is published.
        private async void OnItemChanged()
        {
            await LoadDepartmentsAsync();
            StateHasChanged();
        }

        private async Task LoadDepartmentsAsync()
        {
            Departments = await _departmentService.GetListAsync();
        }

        private void ShowCreateModal()
        {
            CreateModal.Show();
        }

        private async Task ShowUpdateModal(Guid departmentId)
        {
            await UpdateModal.ShowAsync(departmentId);
        }

        private async Task ShowDetailsModalAsync(Guid departmentId)
        {
            await DetailsModal.ShowAsync(departmentId);
        }

        private async Task ShowDeleteModalAsync(Guid departmentId)
        {
            await DeleteModal.ShowAsync(departmentId);
        }
    }
}
