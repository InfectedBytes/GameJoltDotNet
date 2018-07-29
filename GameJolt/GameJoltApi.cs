using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GameJolt.Services;

namespace GameJolt {
	public class GameJoltApi {
		public const string ApiProtocol = "https://";
		public const string ApiRoot = "api.gamejolt.com/api/game/";
		public const string ApiVersion = "1_2";
		public const string ApiBaseUrl = ApiProtocol + ApiRoot + "v" + ApiVersion;

		private readonly int gameId;
		private readonly string privateKey;
		private readonly MD5 md5 = MD5.Create();
		private readonly HttpClient http = new HttpClient();

		public TimeSpan Timeout {
			get => http.Timeout;
			set => http.Timeout = value;
		}

		public Time Time { get; }
		public Users Users { get; }
		public Sessions Sessions { get; }
		public Scores Scores { get; }
		public Trophies Trophies { get; }
		public Friends Friends { get; }
		public Datastore Datastore { get; }

		public GameJoltApi(int gameId, string privateKey, int timeout = 10) {
			this.gameId = gameId;
			this.privateKey = privateKey;
			Timeout = TimeSpan.FromSeconds(timeout);

			Time = new Time(this);
			Users = new Users(this);
			Sessions = new Sessions(this);
			Scores = new Scores(this);
			Trophies = new Trophies(this);
			Friends = new Friends(this);
			Datastore = new Datastore(this);
		}

		private Response<string> ParseDump(string data) {
			var success = data.StartsWith("SUCCESS");
			data = data.Substring(data.IndexOf('\n') + 1);
			return success ? Response.Create(data) : Response.Failure<string>(data);
		}

		private Response<JSONNode> ParseJson(string data) {
			var json = JSONNode.Parse(data)["response"];
			var success = json["success"].AsBool;
			return success ? Response.Create(json) : Response.Failure<JSONNode>(json["message"].Value);
		}

		public async Task<Response<JSONNode>> GetAsync(string method, IDictionary<string, string> parameters) {
			var response = await http.GetAsync(GetRequestUrl(method, parameters, null, ResponseFormat.Json));
			var data = await response.Content.ReadAsStringAsync();
			return ParseJson(data);
		}

		public async Task<Response<string>> GetDumpAsync(string method, IDictionary<string, string> parameters) {
			var response = await http.GetAsync(GetRequestUrl(method, parameters, null, ResponseFormat.Dump));
			var data = await response.Content.ReadAsStringAsync();
			return ParseDump(data);
		}

		public async Task<Response<JSONNode>> PostAsync(string method, IDictionary<string, string> parameters,
			IDictionary<string, string> payload) {
			var response = await http.PostAsync(GetRequestUrl(method, parameters, payload, ResponseFormat.Json),
				new FormUrlEncodedContent(payload ?? Enumerable.Empty<KeyValuePair<string, string>>()));
			var data = await response.Content.ReadAsStringAsync();
			return ParseJson(data);
		}

		public async Task<Response<string>> PostDumpAsync(string method, IDictionary<string, string> parameters,
			IDictionary<string, string> payload) {
			var response = await http.PostAsync(GetRequestUrl(method, parameters, payload, ResponseFormat.Dump),
				new FormUrlEncodedContent(payload ?? Enumerable.Empty<KeyValuePair<string, string>>()));
			var data = await response.Content.ReadAsStringAsync();
			return ParseDump(data);
		}

		private string GetRequestUrl(string method, IDictionary<string, string> parameters,
			IDictionary<string, string> payload, ResponseFormat format) {
			var url = new StringBuilder();
			url.Append(ApiBaseUrl);
			url.Append(method);
			url.Append("?game_id=");
			url.Append(gameId);

			url.Append("&format=");
			url.Append(format.ToString().ToLower());

			if(parameters != null) {
				foreach(var parameter in parameters) {
					url.Append("&");
					url.Append(parameter.Key);
					url.Append("=");
					url.Append(parameter.Value.Replace(" ", "%20"));
				}
			}

			var sb = new StringBuilder();
			sb.Append(url);
			if(payload != null) {
				foreach(var pair in payload.OrderBy(x => x.Key)) {
					sb.Append(pair.Key);
					sb.Append(pair.Value);
				}
			}

			string signature = GetSignature(sb.ToString());
			url.Append("&signature=");
			url.Append(signature);

			return url.ToString();
		}

		private string GetSignature(string input) {
			var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input + privateKey));
			var hashString = new StringBuilder();
			foreach(byte b in hash)
				hashString.Append(b.ToString("x2").ToLower());
			return hashString.ToString().PadLeft(32, '0');
		}
	}
}
