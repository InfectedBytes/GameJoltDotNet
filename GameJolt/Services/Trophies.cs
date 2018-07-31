using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameJolt.Objects;
using GameJolt.Utils;
using JetBrains.Annotations;

namespace GameJolt.Services {
	/// <summary>
	/// Game Jolt allows you to add trophies to your games!
	/// <para>
	/// Trophies come in four materials: bronze, silver, gold, and platinum. 
	/// This is to reflect how difficult it is to achieve a trophy. 
	/// A bronze trophy should be easy to achieve, whereas a platinum trophy should be very hard to achieve.
	/// </para>
	/// On Game Jolt, trophies are always listed in order from easiest to most difficult to achieve.
	/// You can also tag trophies on the site as "secret". 
	/// A sercet trophy's image and description is not visible until a gamer has achieved it.
	/// </summary>
	public sealed class Trophies : Service {
		public Trophies([NotNull] GameJoltApi api) : base(api) { }

		#region Task Api
		/// <summary>
		/// Returns one trophy or multiple trophies, depending on the parameters passed in.
		/// </summary>
		/// <param name="credentials">The user's credentials for which the trophies should be fetched.</param>
		/// <param name="achieved">Pass in true to return only the achieved trophies for a user. 
		/// Pass in false to return only trophies the user hasn't achieved. 
		/// Leave blank (null) to retrieve all trophies.</param>
		/// <param name="ids">You may pass an array of trophy IDs here if you want to return a subset of all the trophies. 
		/// Passing a trophy_id will ignore the <paramref name="achieved"/> parameter if it is passed.</param>
		/// <returns></returns>
		public async Task<Response<Trophy[]>> FetchAsync([NotNull] Credentials credentials, 
			bool? achieved = null, int[] ids = null) {
			credentials.ThrowIfNull();
			var parameters = new Dictionary<string, string> {
				{"username", credentials.Name},
				{"user_token", credentials.Token}
			};
			if(achieved != null)
				parameters.Add("achieved", achieved.Value.ToString().ToLower());
			if(ids != null && ids.Length > 0)
				parameters.Add("trophy_id", string.Join(",", ids));
			var response = await Api.GetAsync("/trophies", parameters);
			return response.Success
				? Response.Create(response.Data["trophies"].ArraySelect(trophy => new Trophy(trophy)))
				: Response.Failure<Trophy[]>(response);
		}

		/// <summary>
		/// Sets a trophy as achieved for a particular user.
		/// </summary>
		/// <param name="credentials">The user's credentials for which the trophy should be unlocked.</param>
		/// <param name="id">The id of the trophy to unlock.</param>
		/// <returns></returns>
		public async Task<Response> SetAchievedAsync([NotNull] Credentials credentials, int id) {
			credentials.ThrowIfNull();
			return await Api.GetAsync("/trophies/add-achieved", new Dictionary<string, string> {
				{"username", credentials.Name},
				{"user_token", credentials.Token},
				{"trophy_id", id.ToString()}
			});
		}

		/// <summary>
		/// Remove a previously achieved trophy for a particular user.
		/// </summary>
		/// <param name="credentials">The user's credentials for which the trophy should be removed.</param>
		/// <param name="id">The id of the trophy to remove.</param>
		/// <returns></returns>
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
		/// <summary>
		/// Returns one trophy or multiple trophies, depending on the parameters passed in.
		/// </summary>
		/// <param name="credentials">The user's credentials for which the trophies should be fetched.</param>
		/// <param name="achieved">Pass in true to return only the achieved trophies for a user. 
		/// Pass in false to return only trophies the user hasn't achieved. 
		/// Leave blank (null) to retrieve all trophies.</param>
		/// <param name="ids">You may pass an array of trophy IDs here if you want to return a subset of all the trophies. 
		/// Passing a trophy_id will ignore the <paramref name="achieved"/> parameter if it is passed.</param>
		/// <param name="callback">Action that is called on completion or error.</param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public void Fetch([NotNull] Credentials credentials, bool? achieved = null, int[] ids = null,
			Action<Response<Trophy[]>> callback = null) {
			Wrap(FetchAsync(credentials, achieved, ids), callback);
		}

		/// <summary>
		/// Sets a trophy as achieved for a particular user.
		/// </summary>
		/// <param name="credentials">The user's credentials for which the trophy should be unlocked.</param>
		/// <param name="id">The id of the trophy to unlock.</param>
		/// <param name="callback">Action that is called on completion or error.</param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public void SetAchieved([NotNull] Credentials credentials, int id, Action<Response> callback = null) {
			Wrap(SetAchievedAsync(credentials, id), callback);
		}

		/// <summary>
		/// Remove a previously achieved trophy for a particular user.
		/// </summary>
		/// <param name="credentials">The user's credentials for which the trophy should be removed.</param>
		/// <param name="id">The id of the trophy to remove.</param>
		/// <param name="callback">Action that is called on completion or error.</param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public void RemoveAchieved([NotNull] Credentials credentials, int id, Action<Response> callback = null) {
			Wrap(RemoveAchievedAsync(credentials, id), callback);
		}
		#endregion
	}
}
