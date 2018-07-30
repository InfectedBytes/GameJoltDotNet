using System;
using System.Threading.Tasks;
using GameJolt.Utils;
using JetBrains.Annotations;

namespace GameJolt.Services {
	public abstract class Service {
		[NotNull] protected GameJoltApi Api { get; }

		protected Service([NotNull] GameJoltApi api) {
			api.ThrowIfNull();
			Api = api;
		}

		protected void Wrap([NotNull] Task<Response> task, Action<Response> callback) {
			if(callback == null) return;
			task.ContinueWith(t => {
				if(t.IsCanceled) callback(Response.Failure(new TimeoutException()));
				else if(t.IsFaulted) callback(Response.Failure(t.Exception.InnerException));
				else callback(t.Result);
			});
		}

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
