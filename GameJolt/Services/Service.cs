using System;
using System.Threading.Tasks;
using GameJolt.Utils;
using JetBrains.Annotations;

namespace GameJolt.Services {
	/// <summary>
	/// Base class for all GameJolt services.
	/// </summary>
	public abstract class Service {
		/// <summary>
		/// API instance which is used to perform http requests.
		/// </summary>
		[NotNull] protected GameJoltApi Api { get; }

		/// <summary>
		/// Constructs a new service by using the provided API instance.
		/// </summary>
		/// <param name="api">The API instance which shall be used for the http requests.</param>
		protected Service([NotNull] GameJoltApi api) {
			api.ThrowIfNull();
			Api = api;
		}

		/// <summary>
		/// Utility function used to provide callback APIs by internally using the async API.
		/// When the provided tasks finishes, the provided callback will be called.
		/// </summary>
		/// <param name="task">The async task which should be wrapped.</param>
		/// <param name="callback">The callback which shall be called when the task finishes.</param>
		[ExcludeFromCodeCoverage]
		protected void Wrap([NotNull] Task<Response> task, Action<Response> callback) {
			if(callback == null) return;
			task.ContinueWith(t => {
				if(t.IsCanceled) callback(Response.Failure(new TimeoutException()));
				else if(t.IsFaulted) callback(Response.Failure(t.Exception.InnerException));
				else callback(t.Result);
			});
		}

		/// <summary>
		/// Utility function used to provide callback APIs by internally using the async API.
		/// When the provided tasks finishes, the provided callback will be called.
		/// </summary>
		/// <param name="task">The async task which should be wrapped.</param>
		/// <param name="callback">The callback which shall be called when the task finishes.</param>
		[ExcludeFromCodeCoverage]
		protected void Wrap<T>([NotNull] Task<Response<T>> task, Action<Response<T>> callback) {
			if(callback == null) return;
			task.ContinueWith(t => {
				if(t.IsCanceled) callback(Response.Failure<T>(new TimeoutException()));
				else if(t.IsFaulted) callback(Response.Failure<T>(t.Exception.InnerException));
				else callback(t.Result);
			});
		}
	}
}
