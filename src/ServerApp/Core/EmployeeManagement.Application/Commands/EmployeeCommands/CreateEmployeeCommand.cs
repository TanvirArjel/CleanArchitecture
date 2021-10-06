using System;
using System.Threading;
using System.Threading.Tasks;
using EmployeeManagement.Domain.Aggregates.EmployeeAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace EmployeeManagement.Application.Commands.EmployeeCommands
{
    public class CreateEmployeeCommand : IRequest<Guid>
    {
        public CreateEmployeeCommand(
            string name,
            Guid departmentId,
            DateTime dateOfBirth,
            string email,
            string phoneNumber)
        {
            Name = name;
            DepartmentId = departmentId;
            DateOfBirth = dateOfBirth;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        public string Name { get; }

        public Guid DepartmentId { get; }

        public DateTime DateOfBirth { get; }

        public string Email { get; }

        public string PhoneNumber { get; }

        private class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Guid>
        {
            private readonly EmployeeFactory _employeeFactory;
            private readonly IEmployeeRepository _employeeRepository;

            public CreateEmployeeCommandHandler(IEmployeeRepository employeeRepository, EmployeeFactory employeeFactory)
            {
                _employeeRepository = employeeRepository;
                _employeeFactory = employeeFactory;
            }

            public async Task<Guid> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
            {
                request.ThrowIfNull(nameof(request));
                Employee employee = await _employeeFactory.CreateAsync(
                    request.Name,
                    request.DepartmentId,
                    request.DateOfBirth,
                    request.Email,
                    request.PhoneNumber);

                await _employeeRepository.InsertAsync(employee);
                return employee.Id;
            }
        }
    }
}
