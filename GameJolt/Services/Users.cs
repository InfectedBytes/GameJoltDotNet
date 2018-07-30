using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameJolt.Objects;
using GameJolt.Utils;
using JetBrains.Annotations;

namespace GameJolt.Services {
	public sealed class Users : Service {
		public Users([NotNull] GameJoltApi api) : base(api) { }

		#region Task Api
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

		public async Task<Response<User>> FetchAsync([NotNull] string name) {
			name.ThrowIfNullOrEmpty();
			var response = await Api.GetAsync("/users", new Dictionary<string, string> { { "username", name } });
			return response.Success ? Response.Create(new User(response.Data["users"][0])) : Response.Failure<User>(response);
		}

		public async Task<Response<User>> FetchAsync(int id) {
			var response = await Api.GetAsync("/users", new Dictionary<string, string> { { "user_id", id.ToString() } });
			return response.Success ? Response.Create(new User(response.Data["users"][0])) : Response.Failure<User>(response);
		}

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
		[ExcludeFromCodeCoverage]
		public void Auth([NotNull] string name, [NotNull] string token, [NotNull] Action<Response<Credentials>> callback) {
			Wrap(AuthAsync(name, token), callback);
		}

		[ExcludeFromCodeCoverage]
		public void Fetch([NotNull] string name, [NotNull] Action<Response<User>> callback) {
			Wrap(FetchAsync(name), callback);
		}

		[ExcludeFromCodeCoverage]
		public void Fetch(int id, [NotNull] Action<Response<User>> callback) {
			Wrap(FetchAsync(id), callback);
		}

		[ExcludeFromCodeCoverage]
		public void Fetch([NotNull] int[] ids, [NotNull] Action<Response<User[]>> callback) {
			Wrap(FetchAsync(ids), callback);
		}
		#endregion
	}
}
