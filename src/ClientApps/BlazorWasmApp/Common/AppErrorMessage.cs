namespace BlazorWasmApp.Common
{
    public static class AppErrorMessage
    {
        public const string ServerErrorMessage = "Server Error: There is a problem with the service. Please try again." +
            " If the problem persists then contact with the system admin.";

        public const string ClientErrorMessage = "Client Error: There is a problem with the service. Please try again." +
            " If the problem persists then contact with the system admin.";

        public const string UnAuthorizedErrorMessage = "You are not authorized or do not have enough permission to complete this request.";
    }
}
