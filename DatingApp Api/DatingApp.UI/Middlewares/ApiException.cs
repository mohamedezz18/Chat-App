namespace API.Middleware
{
    internal class ApiException
    {
        private int statusCode;
        private string message;
        private string StackTrace;

        public ApiException(int statusCode, string message, string v)
        {
            this.statusCode = statusCode;
            this.message = message;
            this.StackTrace = v;
        }
    }
}