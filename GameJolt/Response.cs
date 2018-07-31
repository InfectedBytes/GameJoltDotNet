using System;
using GameJolt.Utils;

namespace GameJolt {
	/// <summary>
	/// Basic response from GameJolt.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public class Response {
		/// <summary>
		/// Whether the request has succeeded or not.
		/// </summary>
		public bool Success { get; }

		/// <summary>
		/// Message in case of error.
		/// </summary>
		public string Message { get; }

		/// <summary>
		/// If the request failed due to an exception, the exception can be retrieved here.
		/// </summary>
		public Exception Exception { get; }

		protected Response(bool success, string message, Exception exception) {
			Success = success;
			Message = message;
			Exception = exception;
		}

		#region Response
		/// <summary>
		/// Instantiates a new failed response with the given message.
		/// </summary>
		/// <param name="message">The error message.</param>
		/// <returns></returns>
		public static Response Failure(string message) {
			return new Response(false, message, null);
		}

		/// <summary>
		/// Instantiates a new failed response with the given exception.
		/// The <see cref="Message"/> field is set to the exceptions message.
		/// </summary>
		/// <param name="exception">The cause of the failed request.</param>
		/// <returns></returns>
		public static Response Failure(Exception exception) {
			return new Response(false, exception.Message, exception);
		}

		/// <summary>
		/// Instantiates a new failed response based on the given response.
		/// </summary>
		/// <param name="response">The original failed response.</param>
		/// <returns></returns>
		public static Response Failure(Response response) {
			return new Response(response.Success, response.Message, response.Exception);
		}
		#endregion

		#region Response<T>
		/// <summary>
		/// Instantiates a new failed response with the given message.
		/// </summary>
		/// <param name="message">The error message.</param>
		/// <returns></returns>
		public static Response<T> Failure<T>(string message) {
			return new Response<T>(false, message, null, default(T));
		}

		/// <summary>
		/// Instantiates a new failed response with the given exception.
		/// The <see cref="Message"/> field is set to the exceptions message.
		/// </summary>
		/// <param name="exception">The cause of the failed request.</param>
		/// <returns></returns>
		public static Response<T> Failure<T>(Exception exception) {
			return new Response<T>(false, exception.Message, exception, default(T));
		}

		/// <summary>
		/// Instantiates a new failed response based on the given response.
		/// </summary>
		/// <param name="response">The original failed response.</param>
		/// <returns></returns>
		public static Response<T> Failure<T>(Response response) {
			return new Response<T>(false, response.Message, response.Exception, default(T));
		}

		/// <summary>
		/// Creates a succeeded response providing the given data.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="data"></param>
		/// <returns></returns>
		public static Response<T> Create<T>(T data) {
			return new Response<T>(true, null, null, data);
		}
		#endregion
	}

	/// <summary>
	/// Generic response providing a certain data object.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[ExcludeFromCodeCoverage]
	public sealed class Response<T> : Response {
		public T Data { get; }

		internal Response(bool success, string message, Exception exception, T data) : base(success, message, exception) {
			Data = data;
		}
	}
}
