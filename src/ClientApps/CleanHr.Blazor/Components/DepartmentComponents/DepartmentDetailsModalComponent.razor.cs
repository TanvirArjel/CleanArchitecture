using System;
using System.Threading.Tasks;
using CleanHr.Blazor.Models.DepartmentModels;
using CleanHr.Blazor.Services;

namespace CleanHr.Blazor.Components.DepartmentComponents;

public partial class DepartmentDetailsModalComponent(DepartmentService departmentService)
{
    private readonly DepartmentService _departmentService = departmentService;

    private string ModalClass { get; set; } = string.Empty;

    private bool ShowBackdrop { get; set; }

    private DepartmentDetailsModel DepartmentDetailsModel { get; set; }

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
}
