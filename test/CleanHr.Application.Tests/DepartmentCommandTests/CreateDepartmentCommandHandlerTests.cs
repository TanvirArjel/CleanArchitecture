using System.Formats.Asn1;
using CleanHr.Application.Caching.Handlers;
using CleanHr.Application.Commands.DepartmentCommands;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using CleanHr.Domain.Exceptions;
using Moq;

namespace CleanHr.Application.Tests;

public class CreateDepartmentCommandHandlerTests
{
	// Arrange
	private readonly Mock<IDepartmentRepository> mockDepartmentRepository = new Mock<IDepartmentRepository>();
	private readonly Mock<IDepartmentCacheHandler> mockDepartmentCacheHandler = new Mock<IDepartmentCacheHandler>();

	[Fact]
	public async Task Handle_WithValidCommand_ReturnsDepartmentId()
	{
		// Act
		CreateDepartmentCommandHandler handler = new CreateDepartmentCommandHandler(
			mockDepartmentRepository.Object, mockDepartmentCacheHandler.Object);

		CreateDepartmentCommand createDepartmentRequest = new CreateDepartmentCommand("Human Resource", "This is human resource department.");
		Guid departmentId = await handler.Handle(createDepartmentRequest, CancellationToken.None);

		// Assert
		Assert.NotEqual(Guid.Empty, departmentId);
		mockDepartmentRepository.Verify(dr => dr.InsertAsync(It.IsAny<Department>()), Times.Once());
		mockDepartmentCacheHandler.Verify(dr => dr.RemoveListAsync(), Times.Once());
	}

	[Theory]
	[InlineData("", "This is human resource department.")]
	[InlineData("HR", "This is human resource department.")]
	[InlineData(null, "This is human resource department.")]
	public async Task Handle_WithInvalidCommand_ReturnsDepartmentId(string departmentName, string description)
	{
		// Act
		CreateDepartmentCommandHandler handler = new CreateDepartmentCommandHandler(
			mockDepartmentRepository.Object, mockDepartmentCacheHandler.Object);

		CreateDepartmentCommand createDepartmentRequest = new CreateDepartmentCommand(departmentName, description);
		await Assert.ThrowsAsync<DomainValidationException>(() => handler.Handle(createDepartmentRequest, CancellationToken.None));
	}
}
