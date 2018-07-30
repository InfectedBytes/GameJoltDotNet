using System;

namespace GameJolt {
	public class Response {
		public bool Success { get; }
		public string Message { get; }
		public Exception Exception { get; }

		protected Response(bool success, string message, Exception exception) {
			Success = success;
			Message = message;
			Exception = exception;
		}

		#region Response
		public static Response Failure(string message) {
			return new Response(false, message, null);
		}

		public static Response Failure(Exception exception) {
			return new Response(false, exception.Message, exception);
		}

		public static Response Failure(Response response) {
			return new Response(response.Success, response.Message, response.Exception);
		}
		#endregion

		#region Response<T>
		public static Response<T> Failure<T>(string message) {
			return new Response<T>(false, message, null, default(T));
		}

		public static Response<T> Failure<T>(Exception exception) {
			return new Response<T>(false, exception.Message, exception, default(T));
		}

		public static Response<T> Failure<T>(Response response) {
			return new Response<T>(false, response.Message, response.Exception, default(T));
		}

		public static Response<T> Create<T>(T data) {
			return new Response<T>(true, null, null, data);
		}
		#endregion
	}

	public sealed class Response<T> : Response {
		public T Data { get; }

		internal Response(bool success, string message, Exception exception, T data) : base(success, message, exception) {
			Data = data;
		}
	}
}
