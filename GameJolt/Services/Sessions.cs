﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameJolt.Objects;
using GameJolt.Utils;
using JetBrains.Annotations;

namespace GameJolt.Services
{
	public sealed class Sessions : Service {
		public Sessions([NotNull] GameJoltApi api) : base(api) { }

		#region Task Api
		public async Task<Response> OpenAsync([NotNull] Credentials credentials) {
			credentials.ThrowIfNull();
			return await Api.GetAsync("/sessions/open", new Dictionary<string, string> {
				{"username", credentials.Name},
				{"user_token", credentials.Token}
			});
		}

		public async Task<Response> PingAsync([NotNull] Credentials credentials, bool active) {
			credentials.ThrowIfNull();
			return await Api.GetAsync("/sessions/ping", new Dictionary<string, string> {
				{"username", credentials.Name},
				{"user_token", credentials.Token},
				{"status", active ? "active" : "idle"}
			});
		}

		public async Task<Response> CheckAsync([NotNull] Credentials credentials) {
			credentials.ThrowIfNull();
			return await Api.GetAsync("/sessions/check", new Dictionary<string, string> {
				{"username", credentials.Name},
				{"user_token", credentials.Token}
			});
		}

		public async Task<Response> CloseAsync([NotNull] Credentials credentials) {
			credentials.ThrowIfNull();
			return await Api.GetAsync("/sessions/close", new Dictionary<string, string> {
				{"username", credentials.Name},
				{"user_token", credentials.Token}
			});
		}
		#endregion

		#region Callback Api
		[ExcludeFromCodeCoverage]
		public void Open([NotNull] Credentials credentials, Action<Response> callback) {
			Wrap(OpenAsync(credentials), callback);
		}

		[ExcludeFromCodeCoverage]
		public void Ping([NotNull] Credentials credentials, bool active, Action<Response> callback) {
			Wrap(PingAsync(credentials, active), callback);
		}

		[ExcludeFromCodeCoverage]
		public void Check([NotNull] Credentials credentials, [NotNull] Action<Response> callback) {
			Wrap(CheckAsync(credentials), callback);
		}

		[ExcludeFromCodeCoverage]
		public void Close([NotNull] Credentials credentials, Action<Response> callback) {
			Wrap(CloseAsync(credentials), callback);
		}
		#endregion
	}
}
