using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameJolt.Objects;
using GameJolt.Utils;
using JetBrains.Annotations;

namespace GameJolt.Services {
	public sealed class Scores : Service {
		public Scores([NotNull] GameJoltApi api) : base(api) { }

		#region Task Api
		public async Task<Response<Score[]>> FetchAsync(Credentials credentials = null, int tableId = 0, int limit = 10,
			int? betterThan = null, int? worseThan = null) {
			if(betterThan != null && worseThan != null)
				throw new ArgumentException(
					$"The {nameof(betterThan)} and {nameof(worseThan)} parameters are both set, but at most one of them is allowed");
			var parameters = new Dictionary<string, string> {{"limit", limit.ToString()}};
			if(credentials != null) {
				parameters.Add("username", credentials.Name);
				parameters.Add("user_token", credentials.Token);
			}
			if(tableId != 0) parameters.Add("table_id", tableId.ToString());
			if(betterThan != null) parameters.Add("better_than", betterThan.Value.ToString());
			if(worseThan != null) parameters.Add("worse_than", worseThan.Value.ToString());
			var response = await Api.GetAsync("/scores", parameters);
			return response.Success
				? Response.Create(response.Data["scores"].ArraySelect(score => new Score(score)))
				: Response.Failure<Score[]>(response);
		}

		public async Task<Response<Table[]>> FetchTablesAsync() {
			var response = await Api.GetAsync("/scores/tables", null);
			return response.Success 
				? Response.Create(response.Data["tables"].ArraySelect(t => new Table(t))) 
				: Response.Failure<Table[]>(response);
		}

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

		public async Task<Response<int>> GetRankAsync(int value, int tableId = 0) {
			var parameters = new Dictionary<string, string> {{"sort", value.ToString()}};
			if(tableId != 0) parameters.Add("table_id", tableId.ToString());
			var response = await Api.GetAsync("/scores/get-rank", parameters);
			return response.Success ? Response.Create(response.Data["rank"].AsInt) : Response.Failure<int>(response);
		}
		#endregion

		#region Callback Api
		[ExcludeFromCodeCoverage]
		public void Fetch(Action<Response<Score[]>> callback, Credentials credentials = null, int tableId = 0, int limit = 10) {
			Wrap(FetchAsync(credentials, tableId, limit), callback);
		}

		[ExcludeFromCodeCoverage]
		public void FetchTables([NotNull] Action<Response<Table[]>> callback) {
			Wrap(FetchTablesAsync(), callback);
		}

		[ExcludeFromCodeCoverage]
		public void Add([NotNull] Credentials credentials, int value, [NotNull] string text, string extra = "",
			int tableId = 0, Action<Response> callback = null) {
			Wrap(AddAsync(credentials, value, text, extra, tableId), callback);
		}

		[ExcludeFromCodeCoverage]
		public void Add([NotNull] string guestName, int value, [NotNull] string text, string extra = "",
			int tableId = 0, Action<Response> callback = null) {
			Wrap(AddAsync(guestName, value, text, extra, tableId), callback);
		}

		[ExcludeFromCodeCoverage]
		public void GetRank(Action<Response<int>> callback, int value, int tableId = 0) {
			Wrap(GetRankAsync(value, tableId), callback);
		}
		#endregion
	}
}
