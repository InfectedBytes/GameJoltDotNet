using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameJolt.Objects;
using GameJolt.Utils;
using JetBrains.Annotations;

namespace GameJolt.Services {
	public sealed class Trophies : Service {
		public Trophies([NotNull] GameJoltApi api) : base(api) { }

		#region Task Api
		public async Task<Response<Trophy[]>> FetchAsync([NotNull] Credentials credentials, 
			bool? achieved = null, int[] ids = null) {
			credentials.ThrowIfNull();
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
				: Response.Failure<Trophy[]>(response);
		}

		public async Task<Response> SetAchievedAsync([NotNull] Credentials credentials, int id) {
			credentials.ThrowIfNull();
			return await Api.GetAsync("/trophies/add-achieved", new Dictionary<string, string> {
				{"username", credentials.Name},
				{"user_token", credentials.Token},
				{"trophy_id", id.ToString()}
			});
		}

		public async Task<Response> RemoveAchievedAsync([NotNull] Credentials credentials, int id) {
			credentials.ThrowIfNull();
			return await Api.GetAsync("/trophies/remove-achieved", new Dictionary<string, string> {
				{"username", credentials.Name},
				{"user_token", credentials.Token},
				{"trophy_id", id.ToString()}
			});
		}
		#endregion

		#region Callback Api
		[ExcludeFromCodeCoverage]
		public void Fetch([NotNull] Credentials credentials, bool? achieved = null, int[] ids = null,
			Action<Response<Trophy[]>> callback = null) {
			Wrap(FetchAsync(credentials, achieved, ids), callback);
		}

		[ExcludeFromCodeCoverage]
		public void SetAchieved([NotNull] Credentials credentials, int id, Action<Response> callback = null) {
			Wrap(SetAchievedAsync(credentials, id), callback);
		}

		[ExcludeFromCodeCoverage]
		public void RemoveAchieved([NotNull] Credentials credentials, int id, Action<Response> callback = null) {
			Wrap(RemoveAchievedAsync(credentials, id), callback);
		}
		#endregion
	}
}
