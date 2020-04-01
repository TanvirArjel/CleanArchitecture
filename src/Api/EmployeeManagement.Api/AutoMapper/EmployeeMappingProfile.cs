using AutoMapper;
using EmployeeManagement.Api.ApiModels.EmployeeModels;
using EmployeeManagement.Application.Dtos.EmployeeDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
