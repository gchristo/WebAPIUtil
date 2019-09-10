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
		public ResponseType ResponseType { get; set; }

        /// <summary>
        /// Information about the response
        /// </summary>
		public string Message;
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
		public T ReturnValue { get; set; }
	}
}