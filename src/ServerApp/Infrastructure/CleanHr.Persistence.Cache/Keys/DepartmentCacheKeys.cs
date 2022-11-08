using System;

namespace CleanHr.Persistence.Cache.Keys;

internal static class DepartmentCacheKeys
{
    public static string ListKey => "DepartmentList";

    public static string SelectListKey => "DepartmentSelectList";

    public static string GetKey(Guid departmentId)
    {
        return $"Department-{departmentId}";
    }

    public static string GetDetailsKey(Guid departmentId)
    {
        return $"DepartmentDetails-{departmentId}";
    }
}
