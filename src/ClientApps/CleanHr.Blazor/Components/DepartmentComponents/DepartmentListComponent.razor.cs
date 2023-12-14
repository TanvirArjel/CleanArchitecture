using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CleanHr.Blazor.Common;
using CleanHr.Blazor.Models.DepartmentModels;
using CleanHr.Blazor.Services;
using TanvirArjel.Blazor.Utilities;

namespace CleanHr.Blazor.Components.DepartmentComponents;

public partial class DepartmentListComponent(
    DepartmentService departmentService,
    ExceptionLogger exceptionLogger)
{
    private readonly DepartmentService _departmentService = departmentService;
    private readonly ExceptionLogger _exceptionLogger = exceptionLogger;

    private List<DepartmentDetailsModel> Departments { get; set; }

    private string ErrorMessage { get; set; }

    private CreateDepartmentModalComponent CreateModal { get; set; }

    private UpdateDepartmentModalComponent UpdateModal { get; set; }

    private DepartmentDetailsModalComponent DetailsModal { get; set; }

    private DeleteDepartmentModalComponent DeleteModal { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadDepartmentsAsync();
    }

    // EventHandler which will be called whenvever ItemUpdated event is published.
    private async void OnItemChanged()
    {
        await LoadDepartmentsAsync();
        StateHasChanged();
    }

    private async Task LoadDepartmentsAsync()
    {
        try
        {
            HttpResponseMessage httpResponse = await _departmentService.GetListAsync();

            if (httpResponse.IsSuccessStatusCode)
            {
                Departments = await httpResponse.Content.ReadFromJsonAsync<List<DepartmentDetailsModel>>();
            }
            else
            {
                if ((int)httpResponse.StatusCode == 401)
                {
                    ErrorMessage = ErrorMessages.Http401ErrorMessage;
                }
                else if ((int)httpResponse.StatusCode == 403)
                {
                    ErrorMessage = ErrorMessages.Http403ErrorMessage;
                }
                else if ((int)httpResponse.StatusCode == 500)
                {
                    ErrorMessage = ErrorMessages.Http500ErrorMessage;
                }
                else
                {
                    ErrorMessage = ErrorMessages.ServerDownOrCorsErrorMessage;
                }
            }
        }
        catch (HttpRequestException exception)
        {
            ErrorMessage = ErrorMessages.ServerDownOrCorsErrorMessage;
            await _exceptionLogger.LogAsync(exception);
        }
        catch (Exception exception)
        {
            await _exceptionLogger.LogAsync(exception);
            ErrorMessage = ErrorMessages.ClientErrorMessage;
        }

        StateHasChanged();
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
