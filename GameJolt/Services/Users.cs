using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using GameJolt.Objects;

namespace GameJolt.Services {
	public sealed class Users : Service {
		public Users(GameJoltApi api) : base(api) { }

		public async Task<Response> AuthAsync(string name, string token) {
			return await Api.GetAsync("/users/auth", new Dictionary<string, string> {
				{"username", name},
				{"user_token", token}
			});
		}

		public void Auth(string name, string token, Action<Response> callback) {
			Contract.Requires(callback != null);
			Wrap(AuthAsync(name, token), callback);
		}

		public async Task<Response<User>> FetchAsync(string name) {
			var response = await Api.GetAsync("/users", new Dictionary<string, string> { { "username", name } });
			return response.Success ? Response.Create(new User(response.Data["users"][0])) : Response.Failure<User>(response.Message);
		}

		public async Task<Response<User>> FetchAsync(int id) {
			var response = await Api.GetAsync("/users", new Dictionary<string, string> { { "user_id", id.ToString() } });
			return response.Success ? Response.Create(new User(response.Data["users"][0])) : Response.Failure<User>(response.Message);
		}

		public async Task<Response<User[]>> FetchAsync(int[] ids) {
			Contract.Requires(ids != null);
			Contract.Requires(ids.Length > 0);
			var idList = string.Join(",", ids);
			var response = await Api.GetAsync("/users", new Dictionary<string, string> { { "user_id", idList } });
			return response.Success
				? Response.Create(response.Data["users"].Childs.Select(user => new User(user)).ToArray())
				: Response.Failure<User[]>(response.Message);
		}

		public void Fetch(string name, Action<Response<User>> callback) {
			Contract.Requires(callback != null);
			Wrap(FetchAsync(name), callback);
		}

		public void Fetch(int id, Action<Response<User>> callback) {
			Contract.Requires(callback != null);
			Wrap(FetchAsync(id), callback);
		}

		public void Fetch(int[] ids, Action<Response<User[]>> callback) {
			Contract.Requires(callback != null);
			Wrap(FetchAsync(ids), callback);
		}
	}
}
