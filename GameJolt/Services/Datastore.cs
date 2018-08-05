using System;
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
		/// <summary>
		/// Adds the value to the current data store item.
		/// </summary>
		Add,
		/// <summary>
		/// Substracts the value from the current data store item.
		/// </summary>
		Subtract,
		/// <summary>
		/// Multiplies the value by the current data store item.
		/// </summary>
		Multiply,
		/// <summary>
		/// Divides the current data store item by the value.
		/// </summary>
		Divide,
		/// <summary>
		/// Appends the value to the current data store item.
		/// </summary>
		Append,
		/// <summary>
		/// Prepends the value to the current data store item.
		/// </summary>
		Prepend
	}

	/// <summary>
	/// A cloud-based data storage system. It's completely up to you what you use this for. The more inventive the better!
	/// Caution: GameJolt has a hard limit of 16MB per key-value pair and a soft limit of 1MB per post-request.
	/// </summary>
	public sealed class Datastore : Service {
		internal Datastore([NotNull] GameJoltApi api) : base(api) { }

		#region Task Api
		/// <summary>
		/// Returns data from the data store. 
		/// </summary>
		/// <param name="key">The key of the data item you'd like to fetch.</param>
		/// <param name="credentials">If provided, the data is retrieved from the user's personal storage.
		/// If left empty, the data is retrieved from the global storage.</param>
		/// <returns></returns>
		public async Task<Response<string>> FetchAsync([NotNull] string key, Credentials credentials = null) {
			key.ThrowIfNullOrEmpty();
			var parameters = new Dictionary<string, string>();
			if(credentials != null) {
				parameters.Add("username", credentials.Name);
				parameters.Add("user_token", credentials.Token);
			}
			return await Api.PostDumpAsync("/data-store", parameters, new Dictionary<string, string> {{"key", key}});
		}

		/// <summary>
		/// Returns either all the keys in the game's global data store, or all the keys in a user's data store.
		/// </summary>
		/// <param name="credentials">If provided, the keys are retrieved from the user's personal storage.
		/// If left empty, the keys are retrieved from the global storage.</param>
		/// <param name="pattern">If you apply a pattern to the request, only keys with applicable key names will be returned. 
		/// The placeholder character for patterns is *.</param>
		/// <returns></returns>
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

		/// <summary>
		/// Sets data in the data store. You can create new data store items by passing in a key that doesn't yet exist in the data store.
		/// Caution: GameJolt has a hard limit of 16MB per key-value pair and a soft limit of 1MB per post-request.
		/// If your data is not url conform, it will be url encoded, which further increases the payload size. 
		/// For example the '+' character will be encoded as "%2B".
		/// </summary>
		/// <param name="key">The key of the data item you'd like to set.</param>
		/// <param name="data">The data you'd like to set.</param>
		/// <param name="credentials">If you pass in the user information, this item will be set in a user's data store. 
		/// If you leave the user information empty, it will be set in the game's global data store.</param>
		/// <returns></returns>
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

		/// <summary>
		/// Removes data from the data store.
		/// </summary>
		/// <param name="key">The key of the data item you'd like to remove.</param>
		/// <param name="credentials">If you pass in the user information, the item will be removed from a user's data store. 
		/// If you leave the user information empty, it will be removed from the game's global data store.</param>
		/// <returns></returns>
		public async Task<Response> RemoveAsync([NotNull] string key, Credentials credentials = null) {
			key.ThrowIfNullOrEmpty();
			var parameters = new Dictionary<string, string>();
			if(credentials != null) {
				parameters.Add("username", credentials.Name);
				parameters.Add("user_token", credentials.Token);
			}
			return await Api.PostDumpAsync("/data-store/remove", parameters, new Dictionary<string, string> {{"key", key}});
		}

		/// <summary>
		/// Updates data in the data store. On success, the new value of the data item is returned.
		/// Caution: GameJolt has a hard limit of 16MB per key-value pair and a soft limit of 1MB per post-request.
		/// </summary>
		/// <param name="key">The key of the data item you'd like to update.</param>
		/// <param name="data">The value you'd like to apply to the data store item. </param>
		/// <param name="operation">The operation you'd like to perform. 
		/// Add, Subtract, Multiply and Divide are only applicable to numeric values.</param>
		/// <param name="credentials">If you pass in the user information, this function will use the user's data store. 
		/// If you leave the user information empty, it will use the game's global data store.</param>
		/// <returns></returns>
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
		/// <summary>
		/// Returns data from the data store. 
		/// </summary>
		/// <param name="key">The key of the data item you'd like to fetch.</param>
		/// <param name="credentials">If provided, the data is retrieved from the user's personal storage.
		/// If left empty, the data is retrieved from the global storage.</param>
		/// <param name="callback">Action that is called on completion or error.</param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public void Fetch([NotNull] string key, Credentials credentials = null, Action<Response<string>> callback = null) {
			Wrap(FetchAsync(key, credentials), callback);
		}

		/// <summary>
		/// Returns either all the keys in the game's global data store, or all the keys in a user's data store.
		/// </summary>
		/// <param name="credentials">If provided, the keys are retrieved from the user's personal storage.
		/// If left empty, the keys are retrieved from the global storage.</param>
		/// <param name="pattern">If you apply a pattern to the request, only keys with applicable key names will be returned. 
		/// The placeholder character for patterns is *.</param>
		/// <param name="callback">Action that is called on completion or error.</param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public void GetKeys(Credentials credentials = null, string pattern = null,
			Action<Response<string[]>> callback = null) {
			Wrap(GetKeysAsync(credentials, pattern), callback);
		}

		/// <summary>
		/// Sets data in the data store. You can create new data store items by passing in a key that doesn't yet exist in the data store.
		/// Caution: GameJolt has a hard limit of 16MB per key-value pair and a soft limit of 1MB per post-request.
		/// If your data is not url conform, it will be url encoded, which further increases the payload size. 
		/// For example the '+' character will be encoded as "%2B".
		/// </summary>
		/// <param name="key">The key of the data item you'd like to set.</param>
		/// <param name="data">The data you'd like to set.</param>
		/// <param name="credentials">If you pass in the user information, this item will be set in a user's data store. 
		/// If you leave the user information empty, it will be set in the game's global data store.</param>
		/// <param name="callback">Action that is called on completion or error.</param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public void Set([NotNull] string key, [NotNull] string data, Credentials credentials = null,
			Action<Response> callback = null) {
			Wrap(SetAsync(key, data, credentials), callback);
		}

		/// <summary>
		/// Removes data from the data store.
		/// </summary>
		/// <param name="key">The key of the data item you'd like to remove.</param>
		/// <param name="credentials">If you pass in the user information, the item will be removed from a user's data store. 
		/// If you leave the user information empty, it will be removed from the game's global data store.</param>
		/// <param name="callback">Action that is called on completion or error.</param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public void Remove([NotNull] string key, Credentials credentials = null, Action<Response> callback = null) {
			Wrap(RemoveAsync(key, credentials), callback);
		}

		/// <summary>
		/// Updates data in the data store. On success, the new value of the data item is returned.
		/// Caution: GameJolt has a hard limit of 16MB per key-value pair and a soft limit of 1MB per post-request.
		/// </summary>
		/// <param name="key">The key of the data item you'd like to update.</param>
		/// <param name="data">The value you'd like to apply to the data store item. </param>
		/// <param name="operation">The operation you'd like to perform. 
		/// Add, Subtract, Multiply and Divide are only applicable to numeric values.</param>
		/// <param name="credentials">If you pass in the user information, this function will use the user's data store. 
		/// If you leave the user information empty, it will use the game's global data store.</param>
		/// <param name="callback">Action that is called on completion or error.</param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public void Update([NotNull] string key, [NotNull] string data,
			DatastoreOperation operation, Credentials credentials = null, Action<Response<string>> callback = null) {
			Wrap(UpdateAsync(key, data, operation, credentials), callback);
		}
		#endregion
	}
}
