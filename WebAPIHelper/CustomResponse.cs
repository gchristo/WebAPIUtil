namespace WebAPIHelper.Models
{
    /// <summary>
    /// Enum with different kinds of response
    /// </summary>
    public enum ResponseType
    {
        ERROR,
        SUCCESS
    }

    /// <summary>
    /// Base class for any kind of Request
    /// </summary>
    public class CustomResponse
    {
        /// <summary>
        /// Type of the response
        /// </summary>
		public ResponseType ResponseType { get; }

        /// <summary>
        /// Information about the response
        /// </summary>
		public string Message { get; }

        public CustomResponse(ResponseType responseType, string message)
        {
            ResponseType = responseType;
            Message = message;
        }

        public static CustomResponse Error(string message) => new CustomResponse(ResponseType.ERROR, message);

        public static CustomResponse Success(string message) => new CustomResponse(ResponseType.SUCCESS, message);

        public static CustomResponse Success() => Success("OK");
    }

    /// <summary>
    /// Class for more elaborated responses, can hold a return value
    /// </summary>
    /// <typeparam name="T">Type of return value</typeparam>
    public class CustomResponse<T> : CustomResponse
    {
        /// <summary>
        /// Object returned from a request
        /// </summary>
        public T ReturnValue { get; }

        public CustomResponse(ResponseType responseType, string message, T returnValue) : base(responseType, message)
        {
            ReturnValue = returnValue;
        }

        public CustomResponse(CustomResponse cr) : base(cr.ResponseType, cr.Message) { }

        public static CustomResponse<T> Success(T returnValue) => Success(returnValue, "OK");

        public static CustomResponse<T> Success(T returnValue, string message) => new CustomResponse<T>(ResponseType.SUCCESS, message, returnValue);

        public new static CustomResponse<T> Error(string message) => new CustomResponse<T>(ResponseType.ERROR, message, default);
    }
}