using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameJolt.Services
{
	public sealed class Sessions : Service {
		public Sessions(GameJoltApi api) : base(api) { }

		public async Task<Response> OpenAsync(string name, string token) {
			return await Api.GetAsync("/sessions/open", new Dictionary<string, string> {
				{"username", name},
				{"user_token", token}
			});
		}

		public async Task<Response> PingAsync(string name, string token, bool active) {
			return await Api.GetAsync("/sessions/ping", new Dictionary<string, string> {
				{"username", name},
				{"user_token", token},
				{"status", active ? "active" : "idle"}
			});
		}

		public async Task<Response> CheckAsync(string name, string token) {
			return await Api.GetAsync("/sessions/check", new Dictionary<string, string> {
				{"username", name},
				{"user_token", token}
			});
		}

		public async Task<Response> CloseAsync(string name, string token) {
			return await Api.GetAsync("/sessions/close", new Dictionary<string, string> {
				{"username", name},
				{"user_token", token}
			});
		}

		public void Open(string name, string token, Action<Response> callback) {
			Wrap(OpenAsync(name, token), callback);
		}

		public void Ping(string name, string token, bool active, Action<Response> callback) {
			Wrap(PingAsync(name, token, active), callback);
		}

		public void Check(string name, string token, Action<Response> callback) {
			Wrap(CheckAsync(name, token), callback);
		}

		public void Close(string name, string token, Action<Response> callback) {
			Wrap(CloseAsync(name, token), callback);
		}
	}
}
