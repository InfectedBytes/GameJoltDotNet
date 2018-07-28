using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace GameJolt.Services
{
	public sealed class Sessions : Service {
		public Sessions([NotNull] GameJoltApi api) : base(api) { }

		#region Task Api
		public async Task<Response> OpenAsync([NotNull] string name, [NotNull] string token) {
			return await Api.GetAsync("/sessions/open", new Dictionary<string, string> {
				{"username", name},
				{"user_token", token}
			});
		}

		public async Task<Response> PingAsync([NotNull] string name, [NotNull] string token, bool active) {
			return await Api.GetAsync("/sessions/ping", new Dictionary<string, string> {
				{"username", name},
				{"user_token", token},
				{"status", active ? "active" : "idle"}
			});
		}

		public async Task<Response> CheckAsync([NotNull] string name, [NotNull] string token) {
			return await Api.GetAsync("/sessions/check", new Dictionary<string, string> {
				{"username", name},
				{"user_token", token}
			});
		}

		public async Task<Response> CloseAsync([NotNull] string name, [NotNull] string token) {
			return await Api.GetAsync("/sessions/close", new Dictionary<string, string> {
				{"username", name},
				{"user_token", token}
			});
		}
		#endregion

		#region Callback Api
		public void Open([NotNull] string name, [NotNull] string token, Action<Response> callback) {
			Wrap(OpenAsync(name, token), callback);
		}

		public void Ping([NotNull] string name, [NotNull] string token, bool active, Action<Response> callback) {
			Wrap(PingAsync(name, token, active), callback);
		}

		public void Check([NotNull] string name, [NotNull] string token, [NotNull] Action<Response> callback) {
			Wrap(CheckAsync(name, token), callback);
		}

		public void Close([NotNull] string name, [NotNull] string token, Action<Response> callback) {
			Wrap(CloseAsync(name, token), callback);
		}
		#endregion
	}
}
