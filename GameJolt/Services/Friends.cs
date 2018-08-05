using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameJolt.Objects;
using GameJolt.Utils;
using JetBrains.Annotations;

namespace GameJolt.Services {
	/// <summary>
	/// A namespace to get information about users friends on Game Jolt.
	/// </summary>
	public sealed class Friends : Service {
		internal Friends([NotNull] GameJoltApi api) : base(api) { }

		#region Task Api
		/// <summary>
		/// Returns the list of a user's friends.
		/// </summary>
		/// <param name="credentials">The user's credentials.</param>
		/// <returns></returns>
		public async Task<Response<int[]>> FetchAsync([NotNull] Credentials credentials) {
			credentials.ThrowIfNull();
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
		/// <summary>
		/// Returns the list of a user's friends.
		/// </summary>
		/// <param name="credentials">The user's credentials.</param>
		/// <param name="callback">Action that is called on completion or error.</param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public void Fetch([NotNull] Credentials credentials, [NotNull] Action<Response<int[]>> callback) {
			callback.ThrowIfNull();
			Wrap(FetchAsync(credentials), callback);
		}
		#endregion
	}
}
