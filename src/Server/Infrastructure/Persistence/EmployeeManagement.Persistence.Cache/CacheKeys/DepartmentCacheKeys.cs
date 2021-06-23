namespace EmployeeManagement.Persistence.Cache.CacheKeys
{
    internal static class DepartmentCacheKeys
    {
        public static string ListKey => "DepartmentList";

        public static string SelectListKey => "DepartmentSelectList";

        public static string GetKey(int departmentId) => $"Department-{departmentId}";

        public static string GetDetailsKey(int departmentId) => $"DepartmentDetails-{departmentId}";
    }
}
