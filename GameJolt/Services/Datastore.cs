﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameJolt.Objects;
using GameJolt.Utils;
using JetBrains.Annotations;

namespace GameJolt.Services {
	/// <summary>
	/// DataStore operations.
	/// </summary>
	public enum DatastoreOperation {
		Add,
		Subtract,
		Multiply,
		Divide,
		Append,
		Prepend
	}

	public sealed class Datastore : Service {
		public Datastore([NotNull] GameJoltApi api) : base(api) { }

		#region Task Api
		public async Task<Response<string>> FetchAsync([NotNull] string key, Credentials credentials = null) {
			key.ThrowIfNullOrEmpty();
			var parameters = new Dictionary<string, string>();
			if(credentials != null) {
				parameters.Add("username", credentials.Name);
				parameters.Add("user_token", credentials.Token);
			}
			return await Api.PostDumpAsync("/data-store", parameters, new Dictionary<string, string> {{"key", key}});
		}

		public async Task<Response<string[]>> GetKeysAsync(Credentials credentials = null, string pattern = null) {
			var parameters = new Dictionary<string, string>();
			if(credentials != null) {
				parameters.Add("username", credentials.Name);
				parameters.Add("user_token", credentials.Token);
			}
			if(pattern != null)
				parameters.Add("pattern", pattern);
			var response = await Api.GetAsync("/data-store/get-keys", parameters);
			return response.Success
				? Response.Create(response.Data["keys"].ArraySelect(key => key["key"].Value))
				: Response.Failure<string[]>(response);
		}

		public async Task<Response> SetAsync([NotNull] string key, [NotNull] string data, Credentials credentials = null) {
			key.ThrowIfNullOrEmpty();
			data.ThrowIfNull();
			var parameters = new Dictionary<string, string>();
			if(credentials != null) {
				parameters.Add("username", credentials.Name);
				parameters.Add("user_token", credentials.Token);
			}
			return await Api.PostDumpAsync("/data-store/set", parameters, new Dictionary<string, string> {
				{"key", key},
				{"data", data}
			});
		}

		public async Task<Response> RemoveAsync([NotNull] string key, Credentials credentials = null) {
			key.ThrowIfNullOrEmpty();
			var parameters = new Dictionary<string, string>();
			if(credentials != null) {
				parameters.Add("username", credentials.Name);
				parameters.Add("user_token", credentials.Token);
			}
			return await Api.PostDumpAsync("/data-store/remove", parameters, new Dictionary<string, string> {{"key", key}});
		}

		public async Task<Response<string>> UpdateAsync([NotNull] string key, [NotNull] string data, 
			DatastoreOperation operation, Credentials credentials = null) {
			key.ThrowIfNullOrEmpty();
			data.ThrowIfNull();
			var parameters = new Dictionary<string, string> {{"operation", operation.ToString().ToLower()}};
			if(credentials != null) {
				parameters.Add("username", credentials.Name);
				parameters.Add("user_token", credentials.Token);
			}
			return await Api.PostDumpAsync("/data-store/update", parameters, new Dictionary<string, string> {
				{"key", key},
				{"value", data}
			});
		}
		#endregion

		#region Callback Api
		[ExcludeFromCodeCoverage]
		public void Fetch([NotNull] string key, Credentials credentials = null, Action<Response<string>> callback = null) {
			Wrap(FetchAsync(key, credentials), callback);
		}

		[ExcludeFromCodeCoverage]
		public void GetKeys(Credentials credentials = null, string pattern = null,
			Action<Response<string[]>> callback = null) {
			Wrap(GetKeysAsync(credentials, pattern), callback);
		}

		[ExcludeFromCodeCoverage]
		public void Remove([NotNull] string key, Credentials credentials = null, Action<Response> callback = null) {
			Wrap(RemoveAsync(key, credentials), callback);
		}

		[ExcludeFromCodeCoverage]
		public void Set([NotNull] string key, [NotNull] string data, Credentials credentials = null,
			Action<Response> callback = null) {
			Wrap(SetAsync(key, data, credentials), callback);
		}

		[ExcludeFromCodeCoverage]
		public void Update([NotNull] string key, [NotNull] string data,
			DatastoreOperation operation, Credentials credentials = null, Action<Response<string>> callback = null) {
			Wrap(UpdateAsync(key, data, operation, credentials), callback);
		}
		#endregion
	}
}
