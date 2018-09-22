using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameJolt.Objects;
using GameJolt.Utils;
using JetBrains.Annotations;

namespace GameJolt.Services {
	/// <summary>
	/// Game Jolt supports multiple online score tables, or scoreboards, per game. 
	/// You are able to, for example, have a score table for each level in your game, or a table for different scoring metrics. 
	/// Gamers will keep coming back to try to achieve the highest scores for your game.
	/// With multiple formatting and sorting options, the system is quite flexible. 
	/// You are also able to include extra data with each score. 
	/// If there is other data associated with the score such as time played, coins collected, etc., you 
	/// should definitely include it. 
	/// It will be helpful in cases where you believe a gamer has illegitimately achieved a high score.
	/// </summary>
	public sealed class Scores : Service {
		internal Scores([NotNull] GameJoltApi api) : base(api) { }

		#region Task Api
		/// <summary>
		/// Returns a list of scores either for a user or globally for a game.
		/// The <paramref name="betterThan"/> and <paramref name="worseThan"/> parameters are mutually exclusive.
		/// </summary>
		/// <param name="credentials">If provided, only the scores of this user are returned.</param>
		/// <param name="tableId">The id of the score table. If left empty, the primary table is used.</param>
		/// <param name="limit">The number of scores you'd like to return. 
		/// The maximum amount of scores you can retrieve is 100.</param>
		/// <param name="betterThan">If provided, only scores better than this value are returned.</param>
		/// <param name="worseThan">If provided, only scores worse than this value are returned.</param>
		public Task<Response<Score[]>> FetchAsync(Credentials credentials = null, int tableId = 0, int limit = 10,
			int? betterThan = null, int? worseThan = null) {
			return FetchAsync(credentials, null, tableId, limit, betterThan, worseThan);
		}

		/// <summary>
		/// Returns a list of scores either for a user or globally for a game.
		/// The <paramref name="betterThan"/> and <paramref name="worseThan"/> parameters are mutually exclusive.
		/// </summary>
		/// <param name="guestName">Only the scores of this guest are returned.</param>
		/// <param name="tableId">The id of the score table. If left empty, the primary table is used.</param>
		/// <param name="limit">The number of scores you'd like to return. 
		/// The maximum amount of scores you can retrieve is 100.</param>
		/// <param name="betterThan">If provided, only scores better than this value are returned.</param>
		/// <param name="worseThan">If provided, only scores worse than this value are returned.</param>
		public Task<Response<Score[]>> FetchAsync(string guestName, int tableId = 0, int limit = 10,
			int? betterThan = null, int? worseThan = null) {
			return FetchAsync(null, guestName, tableId, limit, betterThan, worseThan);
		}

		private async Task<Response<Score[]>> FetchAsync(Credentials credentials, string guestName, int tableId, int limit,
			int? betterThan, int? worseThan) {
			if(credentials != null && guestName != null)
				throw new ArgumentException(
					$"The {nameof(credentials)} and {nameof(guestName)} parameters are both set, but at most one of them is allowed");
			if(betterThan != null && worseThan != null)
				throw new ArgumentException(
					$"The {nameof(betterThan)} and {nameof(worseThan)} parameters are both set, but at most one of them is allowed");
			var parameters = new Dictionary<string, string> { { "limit", limit.ToString() } };
			if(credentials != null) {
				parameters.Add("username", credentials.Name);
				parameters.Add("user_token", credentials.Token);
			} else if(guestName != null) {
				parameters.Add("guest", guestName);
			}
			if(tableId != 0) parameters.Add("table_id", tableId.ToString());
			if(betterThan != null) parameters.Add("better_than", betterThan.Value.ToString());
			if(worseThan != null) parameters.Add("worse_than", worseThan.Value.ToString());
			var response = await Api.GetAsync("/scores", parameters);
			return response.Success
				? Response.Create(response.Data["scores"].ArraySelect(score => new Score(score)))
				: Response.Failure<Score[]>(response);
		}

		/// <summary>
		/// Returns a list of high score tables for a game.
		/// </summary>
		/// <returns></returns>
		public async Task<Response<Table[]>> FetchTablesAsync() {
			var response = await Api.GetAsync("/scores/tables", null);
			return response.Success 
				? Response.Create(response.Data["tables"].ArraySelect(t => new Table(t))) 
				: Response.Failure<Table[]>(response);
		}

		/// <summary>
		/// Adds a score for a user or guest.
		/// </summary>
		/// <param name="credentials">The user's credentials.</param>
		/// <param name="value">This is a numerical sorting value associated with the score. 
		/// All sorting will be based on this number. </param>
		/// <param name="text">This is a string value associated with the score. 
		/// This value will be shown to the user on GameJolt</param>
		/// <param name="extra">If there's any extra data you would like to store as a string, you can use this variable.</param>
		/// <param name="tableId">The ID of the score table to submit to. If not set, the primary table is used.</param>
		/// <returns></returns>
		public async Task<Response> AddAsync([NotNull] Credentials credentials, int value, [NotNull] string text, 
			string extra = "", int tableId = 0) {
			credentials.ThrowIfNull();
			text.ThrowIfNullOrEmpty();
			var parameters = new Dictionary<string, string> {
				{"username", credentials.Name},
				{"user_token", credentials.Token},
				{"sort", value.ToString()},
				{"score", text},
				{"extra_data", extra ?? string.Empty}
			};
			if(tableId != 0) parameters.Add("table_id", tableId.ToString());
			return await Api.GetAsync("/scores/add", parameters);
		}

		/// <summary>
		/// Adds a score for a user or guest. GameJolt let's you define if guest scores are allowed or not.
		/// </summary>
		/// <param name="guestName">The guest's name.</param>
		/// <param name="value">This is a numerical sorting value associated with the score. 
		/// All sorting will be based on this number. </param>
		/// <param name="text">This is a string value associated with the score. 
		/// This value will be shown to the user on GameJolt</param>
		/// <param name="extra">If there's any extra data you would like to store as a string, you can use this variable.</param>
		/// <param name="tableId">The ID of the score table to submit to. If not set, the primary table is used.</param>
		/// <returns></returns>
		public async Task<Response> AddAsync([NotNull] string guestName, int value, [NotNull] string text, 
			string extra = "", int tableId = 0) {
			guestName.ThrowIfNullOrEmpty();
			text.ThrowIfNullOrEmpty();
			var parameters = new Dictionary<string, string> {
				{"guest", guestName},
				{"sort", value.ToString()},
				{"score", text},
				{"extra_data", extra ?? string.Empty}
			};
			if(tableId != 0) parameters.Add("table_id", tableId.ToString());
			return await Api.GetAsync("/scores/add", parameters);
		}

		/// <summary>
		/// Returns the rank of a particular score on a score table.
		/// </summary>
		/// <param name="value">This is a numerical sorting value that is represented by a rank on the score table.</param>
		/// <param name="tableId">The ID of the score table to submit to. If not set, the primary table is used.</param>
		/// <returns></returns>
		public async Task<Response<int>> GetRankAsync(int value, int tableId = 0) {
			var parameters = new Dictionary<string, string> {{"sort", value.ToString()}};
			if(tableId != 0) parameters.Add("table_id", tableId.ToString());
			var response = await Api.GetAsync("/scores/get-rank", parameters);
			return response.Success ? Response.Create(response.Data["rank"].AsInt) : Response.Failure<int>(response);
		}
		#endregion

		#region Callback Api
		/// <summary>
		/// Returns a list of scores either for a user or globally for a game.
		/// </summary>
		/// <param name="credentials">If provided, only the scores of this user are returned.</param>
		/// <param name="tableId">The id of the score table. If left empty, the primary table is used.</param>
		/// <param name="limit">The number of scores you'd like to return. 
		/// The maximum amount of scores you can retrieve is 100.</param>
		/// <param name="betterThan">If provided, only scores better than this value are returned.</param>
		/// <param name="worseThan">If provided, only scores worse than this value are returned.</param>
		/// <param name="callback">Action that is called on completion or error.</param>
		[ExcludeFromCodeCoverage]
		public void Fetch(Credentials credentials = null, int tableId = 0, int limit = 10,
			int? betterThan = null, int? worseThan = null, Action<Response<Score[]>> callback = null) {
			Wrap(FetchAsync(credentials, tableId, limit, betterThan, worseThan), callback);
		}

		/// <summary>
		/// Returns a list of scores either for a guest.
		/// </summary>
		/// <param name="guest">Only the scores of this guest are returned.</param>
		/// <param name="tableId">The id of the score table. If left empty, the primary table is used.</param>
		/// <param name="limit">The number of scores you'd like to return. 
		/// The maximum amount of scores you can retrieve is 100.</param>
		/// <param name="betterThan">If provided, only scores better than this value are returned.</param>
		/// <param name="worseThan">If provided, only scores worse than this value are returned.</param>
		/// <param name="callback">Action that is called on completion or error.</param>
		[ExcludeFromCodeCoverage]
		public void Fetch(string guest, int tableId = 0, int limit = 10,
			int? betterThan = null, int? worseThan = null, Action<Response<Score[]>> callback = null) {
			Wrap(FetchAsync(guest, tableId, limit, betterThan, worseThan), callback);
		}

		/// <summary>
		/// Returns a list of high score tables for a game.
		/// </summary>
		/// <param name="callback">Action that is called on completion or error.</param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public void FetchTables([NotNull] Action<Response<Table[]>> callback) {
			callback.ThrowIfNull();
			Wrap(FetchTablesAsync(), callback);
		}

		/// <summary>
		/// Adds a score for a user or guest.
		/// </summary>
		/// <param name="credentials">The user's credentials.</param>
		/// <param name="value">This is a numerical sorting value associated with the score. 
		/// All sorting will be based on this number. </param>
		/// <param name="text">This is a string value associated with the score. 
		/// This value will be shown to the user on GameJolt</param>
		/// <param name="extra">If there's any extra data you would like to store as a string, you can use this variable.</param>
		/// <param name="tableId">The ID of the score table to submit to. If not set, the primary table is used.</param>
		/// <param name="callback">Action that is called on completion or error.</param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public void Add([NotNull] Credentials credentials, int value, [NotNull] string text, string extra = "",
			int tableId = 0, Action<Response> callback = null) {
			Wrap(AddAsync(credentials, value, text, extra, tableId), callback);
		}

		/// <summary>
		/// Adds a score for a user or guest. GameJolt let's you define if guest scores are allowed or not.
		/// </summary>
		/// <param name="guestName">The guest's name.</param>
		/// <param name="value">This is a numerical sorting value associated with the score. 
		/// All sorting will be based on this number. </param>
		/// <param name="text">This is a string value associated with the score. 
		/// This value will be shown to the user on GameJolt</param>
		/// <param name="extra">If there's any extra data you would like to store as a string, you can use this variable.</param>
		/// <param name="tableId">The ID of the score table to submit to. If not set, the primary table is used.</param>
		/// <param name="callback">Action that is called on completion or error.</param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public void Add([NotNull] string guestName, int value, [NotNull] string text, string extra = "",
			int tableId = 0, Action<Response> callback = null) {
			Wrap(AddAsync(guestName, value, text, extra, tableId), callback);
		}

		/// <summary>
		/// Returns the rank of a particular score on a score table.
		/// </summary>
		/// <param name="value">This is a numerical sorting value that is represented by a rank on the score table.</param>
		/// <param name="tableId">The ID of the score table to submit to. If not set, the primary table is used.</param>
		/// <param name="callback">Action that is called on completion or error.</param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public void GetRank(int value, int tableId = 0, Action<Response<int>> callback = null) {
			Wrap(GetRankAsync(value, tableId), callback);
		}
		#endregion
	}
}
