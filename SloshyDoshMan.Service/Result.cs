namespace SloshyDoshMan.Service
{
	public class Result
	{
		public bool Success { get; private set; }
		public string ErrorMessage { get; private set; }

		public static Result Failure(string error)
		{
			return new Result { Success = false, ErrorMessage = error };
		}

		public static readonly Result Successful = new Result { Success = true, ErrorMessage = null };
	}

	public class Result<T>
	{
		public bool Success { get; private set; }
		public string ErrorMessage { get; private set; }
		public T Data { get; set; }

		public static Result<T> Successful(T data)
		{
			return new Result<T> { Success = true, Data = data, ErrorMessage = null };
		}

		public static Result<T> Failure(string error)
		{
			return new Result<T> { Success = false, ErrorMessage = error };
		}
	}
}
