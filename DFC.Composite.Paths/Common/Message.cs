namespace DFC.Composite.Paths.Common
{
    public class Message
    {
        public const string PathDoesNotExist = "Path Does Not Exist";
        public const string PayloadMalformed = "Payload is malformed";
        public const string ValidationFailed = "Validation Failed";
        public const string UnableToLocatePathInQueryString = "Unable to locate path in query string";

        public const string ExternalUrlMustUseLayoutNone = "External urls must use Layout.None";
        public const string NonExternalUrlMustNotUseLayoutNone = "Non external urls must not use Layout.None";
        public const string PathAlreadyExists = "A path with the name {0} is already registered";
    }
}
