namespace EmployeeManagement.Infrastructure.Data.CacheKeys
{
    public static class EmployeeCacheKeys
    {
        public static string GetKey(long employeeId) => $"Employee-{employeeId}";

        public static string GetDetailsKey(long employeeId) => $"EmployeeDetails-{employeeId}";
    }
}
