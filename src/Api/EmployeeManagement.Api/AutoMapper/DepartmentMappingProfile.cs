using AutoMapper;
using EmployeeManagement.Api.ApiModels.DepartmentModels;
using EmployeeManagement.Application.Dtos.DepartmentDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
