using System;

namespace GameJolt {
	public class Response {
		public bool Success { get; }
		public string Message { get; }
		public Exception Exception { get; }

		protected Response(bool success, string message) {
			Success = success;
			Message = message;
		}

		protected Response(Exception exception) {
			Success = false;
			Message = exception.Message;
			Exception = exception;
		}

		public static Response Failure(string message) {
			return new Response(false, message);
		}

		public static Response Failure(Exception exception) {
			return new Response(exception);
		}

		public static Response<T> Failure<T>(string message) {
			return new Response<T>(false, message, default(T));
		}

		public static Response<T> Failure<T>(Exception exception) {
			return new Response<T>(exception);
		}

		public static Response<T> Create<T>(T data) {
			return new Response<T>(true, null, data);
		}
	}

	public sealed class Response<T> : Response {
		public T Data { get; }

		internal Response(bool success, string message, T data) : base(success, message) {
			Data = data;
		}

		internal Response(Exception exception) : base(exception) { }
	}
}
