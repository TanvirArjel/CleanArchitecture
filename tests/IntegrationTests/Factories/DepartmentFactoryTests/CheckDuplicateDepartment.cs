using EmployeeManagement.Domain.Aggregates.DepartmentAggregate;
using EmployeeManagement.Persistence.RelationalDB;
using EmployeeManagement.Persistence.RelationalDB.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;


namespace IntegrationTests.Factories.DepartmentFactoryTests
{
    public class CheckDuplicateDepartment
    {
        private readonly EmployeeManagementDbContext _dataContext;
        private readonly DepartmentFactory _departmentFactory;
        private readonly IDepartmentRepository _departmentRepository;

        private string _name = "Software";
        private string _description = "Internal software department";

        public CheckDuplicateDepartment()
        {
            var dbOptions = new DbContextOptionsBuilder<EmployeeManagementDbContext>()
                //.UseSqlite("Filename=TestEmpManagement.db")
                .UseInMemoryDatabase(databaseName: "TestEmpManagement")
                .Options;
            _dataContext = new EmployeeManagementDbContext(dbOptions);
            _departmentRepository = new DepartmentRepository(_dataContext);
            _departmentFactory = new DepartmentFactory(_departmentRepository);
        }

        [Fact]
        public async Task Should_Throws_InvalidOperationException_When_DepartmentNameDuplicate()
        {
            var department = await _departmentFactory.CreateAsync(_name, _description);
            await _departmentRepository.InsertAsync(department);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _departmentFactory.CreateAsync(_name, _description));
        }
    }
}
