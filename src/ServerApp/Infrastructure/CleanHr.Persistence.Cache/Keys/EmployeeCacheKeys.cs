using System;

namespace CleanHr.Persistence.Cache.Keys;

public static class EmployeeCacheKeys
{
    public static string GetKey(Guid employeeId)
    {
        return $"Employee-{employeeId}";
    }

    public static string GetDetailsKey(Guid employeeId)
    {
        return $"EmployeeDetails-{employeeId}";
    }
}
