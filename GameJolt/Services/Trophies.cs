using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameJolt.Objects;
using GameJolt.Utils;
using JetBrains.Annotations;

namespace GameJolt.Services {
	public sealed class Trophies : Service {
		public Trophies(GameJoltApi api) : base(api) { }

		#region Task Api
		public async Task<Response<Trophy[]>> FetchAsync([NotNull] Credentials credentials, 
			bool? achieved = null, int[] ids = null) {
			var parameters = new Dictionary<string, string> {
				{"username", credentials.Name},
				{"user_token", credentials.Token}
			};
			if(achieved != null)
				parameters.Add("achieved", achieved.Value.ToString().ToLower());
			if(ids != null)
				parameters.Add("trophy_id", string.Join(",", ids));
			var response = await Api.GetAsync("/trophies", parameters);
			return response.Success
				? Response.Create(response.Data["trophies"].ArraySelect(trophy => new Trophy(trophy)))
				: Response.Failure<Trophy[]>(response.Message);
		}

		public async Task<Response> SetAchievedAsync([NotNull] Credentials credentials, int id) {
			return await Api.GetAsync("/trophies/add-achieved", new Dictionary<string, string> {
				{"username", credentials.Name},
				{"user_token", credentials.Token},
				{"trophy_id", id.ToString()}
			});
		}

		public async Task<Response> RemoveAchievedAsync([NotNull] Credentials credentials, int id) {
			return await Api.GetAsync("/trophies/remove-achieved", new Dictionary<string, string> {
				{"username", credentials.Name},
				{"user_token", credentials.Token},
				{"trophy_id", id.ToString()}
			});
		}
		#endregion

		#region Callback Api
		public void Fetch([NotNull] Action<Response<Trophy[]>> callback, [NotNull]  Credentials credentials, 
			bool? achieved = null, int[] ids = null) {
			Wrap(FetchAsync(credentials, achieved, ids), callback);
		}

		public void SetAchieved([NotNull] Credentials credentials, int id, Action<Response> callback) {
			Wrap(SetAchievedAsync(credentials, id), callback);
		}

		public void RemoveAchieved([NotNull] Credentials credentials, int id, Action<Response> callback) {
			Wrap(RemoveAchievedAsync(credentials, id), callback);
		}
		#endregion
	}
}
