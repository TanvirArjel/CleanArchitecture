using System;

namespace EmployeeManagement.Persistence.Cache.CacheKeys
{
    public static class EmployeeCacheKeys
    {
        public static string GetKey(Guid employeeId) => $"Employee-{employeeId}";

        public static string GetDetailsKey(Guid employeeId) => $"EmployeeDetails-{employeeId}";
    }
}
