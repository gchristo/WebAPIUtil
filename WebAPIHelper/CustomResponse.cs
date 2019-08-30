namespace WebAPIHelper.Models
{
	public enum ResponseType
	{
		ERROR,
		SUCCESS
	}

	public class CustomResponse
	{
		public ResponseType ResponseType { get; set; }
		public string Message;
	}

	public class CustomResponse<T> : CustomResponse
	{
		public T ReturnValue { get; set; }
	}
}