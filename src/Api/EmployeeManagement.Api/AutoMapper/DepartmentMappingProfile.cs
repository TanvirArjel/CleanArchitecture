using AutoMapper;
using EmployeeManagement.Api.ApiModels.DepartmentModels;
using EmployeeManagement.Application.Dtos.DepartmentDtos;
using EmployeeManagement.Domain.Dtos.DepartmentDtos;

namespace EmployeeManagement.Api.AutoMapper
{
    public class DepartmentMappingProfile : Profile
    {
        public DepartmentMappingProfile()
        {
            CreateMap<CreateDepartmentModel, CreateDepartmentDto>();
            CreateMap<DepartmentDetailsDto, DepartmentDetailsModel>();
            CreateMap<UpdateDepartmentModel, UpdateDepartmentDto>();
        }
    }
}
