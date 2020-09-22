using AutoMapper;
using EmployeeManagement.Api.ApiModels.EmployeeModels;
using EmployeeManagement.Application.Dtos.EmployeeDtos;

namespace EmployeeManagement.Api.AutoMapper
{
    public class EmployeeMappingProfile : Profile
    {
        public EmployeeMappingProfile()
        {
            CreateMap<CreateEmployeeModel, CreateEmployeeDto>();
            CreateMap<UpdateEmployeeModel, UpdateEmployeeDto>();
            CreateMap<EmployeeDetailsDto, EmployeeDetailsModel>();
        }
    }
}
