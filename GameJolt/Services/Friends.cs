using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameJolt.Objects;
using GameJolt.Utils;

namespace GameJolt.Services {
	public sealed class Friends : Service {
		public Friends(GameJoltApi api) : base(api) { }

		#region Task Api
		public async Task<Response<int[]>> FetchAsync(Credentials credentials) {
			var response = await Api.GetAsync("/friends", new Dictionary<string, string> {
				{"username", credentials.Name},
				{"user_token", credentials.Token}
			});
			return response.Success
				? Response.Create(response.Data["friends"].ArraySelect(friend => friend["friend_id"].AsInt))
				: Response.Failure<int[]>(response);
		}
		#endregion

		#region Callback Api
		public void Fetch(Credentials credentials, Action<Response<int[]>> callback) {
			Wrap(FetchAsync(credentials), callback);
		}
		#endregion
	}
}
