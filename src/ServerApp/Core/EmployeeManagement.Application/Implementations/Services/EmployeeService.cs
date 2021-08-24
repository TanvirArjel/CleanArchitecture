using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EmployeeManagement.Application.CacheRepositories;
using EmployeeManagement.Application.Dtos.EmployeeDtos;
using EmployeeManagement.Application.Exceptions;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Domain.Aggregates.EmployeeAggregate;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace EmployeeManagement.Application.Implementations.Services
{
    internal class EmployeeService : IEmployeeService
    {
        private readonly EmployeeDomainService _employeeDomainService;
        private readonly IEmployeeCacheRepository _employeeCacheRepository;
        private readonly IRepository _repository;

        public EmployeeService(
            IEmployeeCacheRepository employeeCacheRepository,
            IRepository repository,
            EmployeeDomainService employeeDomainService)
        {
            _employeeCacheRepository = employeeCacheRepository;
            _repository = repository;
            _employeeDomainService = employeeDomainService;
        }

        public async Task<PaginatedList<EmployeeDetailsDto>> GetListAsync(int pageNumber, int pageSize)
        {
            pageNumber.ThrowIfOutOfRange(1, 50, nameof(pageNumber));
            pageNumber.ThrowIfOutOfRange(1, 50, nameof(pageSize));

            Expression<Func<Employee, EmployeeDetailsDto>> selectExpression = e => new EmployeeDetailsDto
            {
                Id = e.Id,
                Name = e.Name,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department.Name,
                DateOfBirth = e.DateOfBirth,
                Email = e.Email,
                PhoneNumber = e.PhoneNumber,
                IsActive = e.IsActive,
                CreatedAtUtc = e.CreatedAtUtc,
                LastModifiedAtUtc = e.LastModifiedAtUtc
            };

            PaginationSpecification<Employee> paginationSpecification = new PaginationSpecification<Employee>
            {
                PageIndex = pageNumber,
                PageSize = pageSize
            };

            PaginatedList<EmployeeDetailsDto> employeeDetailsDtos = await _repository.GetListAsync(paginationSpecification, selectExpression);

            return employeeDetailsDtos;
        }

        public async Task<EmployeeDetailsDto> GetDetailsByIdAsync(Guid employeeId)
        {
            employeeId.ThrowIfEmpty(nameof(employeeId));

            EmployeeDetailsDto employeeDetailsDto = await _employeeCacheRepository.GetDetailsByIdAsync(employeeId);

            return employeeDetailsDto;
        }

        public async Task CreateAsync(CreateEmployeeDto createEmployeeDto)
        {
            createEmployeeDto.ThrowIfNull(nameof(createEmployeeDto));

            Employee employeeToBeCreated = await _employeeDomainService.CreateAsync(
                createEmployeeDto.Name,
                createEmployeeDto.DepartmentId,
                createEmployeeDto.DateOfBirth,
                createEmployeeDto.Email,
                createEmployeeDto.PhoneNumber);

            await _repository.InsertAsync(employeeToBeCreated);
        }

        public async Task UpdateAsync(UpdateEmployeeDto updateEmployeeDto)
        {
            try
            {
                updateEmployeeDto.ThrowIfNull(nameof(updateEmployeeDto));

                Employee employeeeToBeUpdated = await _repository.GetByIdAsync<Employee>(updateEmployeeDto.Id);

                if (employeeeToBeUpdated == null)
                {
                    throw new EntityNotFoundException(typeof(Employee), updateEmployeeDto.Id);
                }

                employeeeToBeUpdated.SetName(updateEmployeeDto.Name);
                employeeeToBeUpdated.SetDateOfBirth(updateEmployeeDto.DateOfBirth);

                await _employeeDomainService.SetDepartmentAsync(employeeeToBeUpdated, updateEmployeeDto.DepartmentId);
                await _employeeDomainService.SetEmailAsync(employeeeToBeUpdated, updateEmployeeDto.Email);
                await _employeeDomainService.SetPhoneNumberAsync(employeeeToBeUpdated, updateEmployeeDto.PhoneNumber);

                await _employeeCacheRepository.UpdateAsync(employeeeToBeUpdated);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteAsync(Guid employeeId)
        {
            employeeId.ThrowIfEmpty(nameof(employeeId));

            Employee employeeeToBeDeleted = await _repository.GetByIdAsync<Employee>(employeeId);

            if (employeeeToBeDeleted == null)
            {
                throw new EntityNotFoundException(typeof(Employee), employeeId);
            }

            await _repository.DeleteAsync(employeeeToBeDeleted);
        }
    }
}
