﻿using System.Linq.Expressions;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Queries.DepartmentQueries;

public sealed class GetDepartmentByIdQuery : IRequest<DepartmentDetailsDto>
{
    public GetDepartmentByIdQuery(Guid id)
    {
        Id = id.ThrowIfEmpty(nameof(id));
    }

    public Guid Id { get; }

    // Handler
    private class GetDepartmentByIdQueryHandler : IRequestHandler<GetDepartmentByIdQuery, DepartmentDetailsDto>
    {
        private readonly IQueryRepository _repository;

        public GetDepartmentByIdQueryHandler(IQueryRepository repository)
        {
            _repository = repository;
        }

        public async Task<DepartmentDetailsDto> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            Expression<Func<Department, DepartmentDetailsDto>> selectExp = d => new DepartmentDetailsDto
            {
                Id = d.Id,
                Name = d.Name.Value,
                Description = d.Description,
                IsActive = d.IsActive,
                CreatedAtUtc = d.CreatedAtUtc,
                LastModifiedAtUtc = d.LastModifiedAtUtc
            };

            DepartmentDetailsDto departmentDetailsDto = await _repository.GetByIdAsync(request.Id, selectExp, cancellationToken);

            return departmentDetailsDto;
        }
    }
}

public class DepartmentDetailsDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? LastModifiedAtUtc { get; set; }
}
