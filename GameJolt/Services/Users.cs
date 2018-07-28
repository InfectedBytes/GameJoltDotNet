using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameJolt.Objects;
using JetBrains.Annotations;

namespace GameJolt.Services {
	public sealed class Users : Service {
		public Users([NotNull] GameJoltApi api) : base(api) { }

		#region Task Api
		public async Task<Response> AuthAsync([NotNull] string name, [NotNull] string token) {
			return await Api.GetAsync("/users/auth", new Dictionary<string, string> {
				{"username", name},
				{"user_token", token}
			});
		}

		public async Task<Response<User>> FetchAsync([NotNull] string name) {
			var response = await Api.GetAsync("/users", new Dictionary<string, string> { { "username", name } });
			return response.Success ? Response.Create(new User(response.Data["users"][0])) : Response.Failure<User>(response.Message);
		}

		public async Task<Response<User>> FetchAsync(int id) {
			var response = await Api.GetAsync("/users", new Dictionary<string, string> { { "user_id", id.ToString() } });
			return response.Success ? Response.Create(new User(response.Data["users"][0])) : Response.Failure<User>(response.Message);
		}

		public async Task<Response<User[]>> FetchAsync([NotNull] int[] ids) {
			var idList = string.Join(",", ids);
			var response = await Api.GetAsync("/users", new Dictionary<string, string> { { "user_id", idList } });
			return response.Success
				? Response.Create(response.Data["users"].Childs.Select(user => new User(user)).ToArray())
				: Response.Failure<User[]>(response.Message);
		}
		#endregion

		#region Callback Api
		public void Auth([NotNull] string name, [NotNull] string token, [NotNull] Action<Response> callback) {
			Wrap(AuthAsync(name, token), callback);
		}

		public void Fetch([NotNull] string name, [NotNull] Action<Response<User>> callback) {
			Wrap(FetchAsync(name), callback);
		}

		public void Fetch(int id, [NotNull] Action<Response<User>> callback) {
			Wrap(FetchAsync(id), callback);
		}

		public void Fetch([NotNull] int[] ids, [NotNull] Action<Response<User[]>> callback) {
			Wrap(FetchAsync(ids), callback);
		}
		#endregion
	}
}
