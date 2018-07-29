using System;
using System.Threading.Tasks;

namespace GameJolt.Services {
	public abstract class Service : IService {
		protected GameJoltApi Api { get; }

		protected Service(GameJoltApi api) {
			Api = api;
		}

		protected void Wrap(Task<Response> task, Action<Response> callback) {
			if(callback == null) return;
			task.ContinueWith(t => {
				if(t.IsCanceled) callback(Response.Failure(new TimeoutException()));
				// ReSharper disable once PossibleNullReferenceException
				else if(t.IsFaulted) callback(Response.Failure(t.Exception.InnerException));
				else callback(t.Result);
			});
		}

		protected void Wrap<T>(Task<Response<T>> task, Action<Response<T>> callback) {
			if(callback == null) return;
			task.ContinueWith(t => {
				if(t.IsCanceled) callback(Response.Failure<T>(new TimeoutException()));
				// ReSharper disable once PossibleNullReferenceException
				else if(t.IsFaulted) callback(Response.Failure<T>(t.Exception.InnerException));
				else callback(t.Result);
			});
		}
	}
}
