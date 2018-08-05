using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameJolt.Objects;
using GameJolt.Utils;
using JetBrains.Annotations;

namespace GameJolt.Services {
	/// <summary>
	/// Your games will not authenticate users by using their username and password. 
	/// Instead, users have a token to verify themselves along with their username.
	/// </summary>
	public sealed class Users : Service {
		internal Users([NotNull] GameJoltApi api) : base(api) { }

		#region Task Api
		/// <summary>
		/// Authenticates the user's information. 
		/// This should be done before you make any calls for the user, to make sure the user's credentials (username and token) are valid.
		/// When the returned is awaited, the resulting <see cref="Credentials"/> object can be used to perform user queries.
		/// </summary>
		/// <param name="name">The user's username.</param>
		/// <param name="token">The user's token (not his password!)</param>
		/// <returns></returns>
		public async Task<Response<Credentials>> AuthAsync([NotNull] string name, [NotNull] string token) {
			name.ThrowIfNullOrEmpty();
			token.ThrowIfNullOrEmpty();
			var response = await Api.GetAsync("/users/auth", new Dictionary<string, string> {
				{"username", name},
				{"user_token", token}
			});
			return response.Success
				? Response.Create(new Credentials(name, token))
				: Response.Failure<Credentials>(response);
		}

		/// <summary>
		/// Returns a user's data, like name, id, avatar, etc.
		/// </summary>
		/// <param name="name">The user's username.</param>
		/// <returns></returns>
		public async Task<Response<User>> FetchAsync([NotNull] string name) {
			name.ThrowIfNullOrEmpty();
			var response = await Api.GetAsync("/users", new Dictionary<string, string> { { "username", name } });
			return response.Success ? Response.Create(new User(response.Data["users"][0])) : Response.Failure<User>(response);
		}

		/// <summary>
		/// Returns a user's data, like name, id, avatar, etc.
		/// </summary>
		/// <param name="id">The user's GameJolt id.</param>
		/// <returns></returns>
		public async Task<Response<User>> FetchAsync(int id) {
			var response = await Api.GetAsync("/users", new Dictionary<string, string> { { "user_id", id.ToString() } });
			return response.Success ? Response.Create(new User(response.Data["users"][0])) : Response.Failure<User>(response);
		}

		/// <summary>
		/// Returns the data for several user's at once.
		/// </summary>
		/// <param name="ids">Array of user ids.</param>
		/// <returns></returns>
		public async Task<Response<User[]>> FetchAsync([NotNull] int[] ids) {
			ids.ThrowIfNullOrEmpty();
			var idList = string.Join(",", ids);
			var response = await Api.GetAsync("/users", new Dictionary<string, string> { { "user_id", idList } });
			return response.Success
				? Response.Create(response.Data["users"].Childs.Select(user => new User(user)).ToArray())
				: Response.Failure<User[]>(response);
		}
		#endregion

		#region Callback Api
		/// <summary>
		/// Authenticates the user's information. 
		/// This should be done before you make any calls for the user, to make sure the user's credentials (username and token) are valid.
		/// The callback is called with the authenticated user credentials or null if the authentication has failed.
		/// </summary>
		/// <param name="name">The user's username.</param>
		/// <param name="token">The user's token (not his password!)</param>
		/// <param name="callback">Action that is called on completion or error.</param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public void Auth([NotNull] string name, [NotNull] string token, [NotNull] Action<Response<Credentials>> callback) {
			callback.ThrowIfNull();
			Wrap(AuthAsync(name, token), callback);
		}

		/// <summary>
		/// Returns a user's data, like name, id, avatar, etc.
		/// </summary>
		/// <param name="name">The user's username.</param>
		/// <param name="callback">Action that is called on completion or error.</param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public void Fetch([NotNull] string name, [NotNull] Action<Response<User>> callback) {
			callback.ThrowIfNull();
			Wrap(FetchAsync(name), callback);
		}

		/// <summary>
		/// Returns a user's data, like name, id, avatar, etc.
		/// </summary>
		/// <param name="id">The user's GameJolt id.</param>
		/// <param name="callback">Action that is called on completion or error.</param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public void Fetch(int id, [NotNull] Action<Response<User>> callback) {
			callback.ThrowIfNull();
			Wrap(FetchAsync(id), callback);
		}

		/// <summary>
		/// Returns the data for several user's at once.
		/// </summary>
		/// <param name="ids">Array of user ids.</param>
		/// <param name="callback">Action that is called on completion or error.</param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public void Fetch([NotNull] int[] ids, [NotNull] Action<Response<User[]>> callback) {
			callback.ThrowIfNull();
			Wrap(FetchAsync(ids), callback);
		}
		#endregion
	}
}
