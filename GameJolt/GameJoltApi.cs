using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GameJolt.Services;
using GameJolt.Utils;
using JetBrains.Annotations;

[assembly:InternalsVisibleTo("GameJolt.UnitTests")]

namespace GameJolt {
	/// <summary>
	/// Core of the GameJolt api. Encapsulates all services and stores all relevant settings.
	/// </summary>
	public class GameJoltApi {
		private enum ResponseFormat {Json, Dump}

		private const string ApiProtocol = "https://";
		private const string ApiRoot = "api.gamejolt.com/api/game/";
		private const string ApiVersion = "1_2";
		private const string ApiBaseUrl = ApiProtocol + ApiRoot + "v" + ApiVersion;

		private readonly int gameId;
		private readonly string privateKey;
		private readonly MD5 md5 = MD5.Create();
		private readonly HttpClient http = new HttpClient();

		#region Public Fields
		/// <summary>
		/// Gets or sets the timeout for http requests.
		/// </summary>
		public TimeSpan Timeout {
			get => http.Timeout;
			set => http.Timeout = value;
		}

		/// <summary>
		/// Get the server's time.
		/// </summary>
		[NotNull] public Time Time { get; }

		/// <summary>
		/// Access user-based features.
		/// </summary>
		[NotNull] public Users Users { get; }

		/// <summary>
		/// Set up sessions for your game.
		/// </summary>
		[NotNull] public Sessions Sessions { get; }

		/// <summary>
		/// Manipulate scores on score tables.
		/// </summary>
		[NotNull] public Scores Scores { get; }

		/// <summary>
		/// Manage trophies for your game.
		/// </summary>
		[NotNull] public Trophies Trophies { get; }

		/// <summary>
		/// List a user's friends.
		/// </summary>
		[NotNull] public Friends Friends { get; }

		/// <summary>
		/// Manipulate items in a cloud-based data storage.
		/// </summary>
		[NotNull] public Datastore Datastore { get; }
		#endregion

		/// <summary>
		/// Constructs a new API instance using the provided credentials.
		/// </summary>
		/// <param name="gameId">The id of your game.</param>
		/// <param name="privateKey">The private key of your game.</param>
		/// <param name="timeout">Http request timeout.</param>
		public GameJoltApi(int gameId, [NotNull] string privateKey, int timeout = 10) {
			privateKey.ThrowIfNullOrEmpty();
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

		#region Get/Post Requests
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

		/// <summary>
		/// Executes a GET-request and returns the result as a JSON object.
		/// </summary>
		/// <param name="method">The GameJolt method to invoke.</param>
		/// <param name="parameters">The arguments to pass to GameJolt</param>
		/// <returns></returns>
		internal async Task<Response<JSONNode>> GetAsync([NotNull] string method, IDictionary<string, string> parameters) {
			method.ThrowIfNullOrEmpty();
			var response = await http.GetAsync(GetRequestUrl(method, parameters, null, ResponseFormat.Json));
			var data = await response.Content.ReadAsStringAsync();
			return ParseJson(data);
		}

		/// <summary>
		/// Executes a GET-request and returns the result as a raw string.
		/// </summary>
		/// <param name="method">The GameJolt method to invoke.</param>
		/// <param name="parameters">The arguments to pass to GameJolt</param>
		/// <returns></returns>
		internal async Task<Response<string>> GetDumpAsync([NotNull] string method, IDictionary<string, string> parameters) {
			method.ThrowIfNullOrEmpty();
			var response = await http.GetAsync(GetRequestUrl(method, parameters, null, ResponseFormat.Dump));
			var data = await response.Content.ReadAsStringAsync();
			return ParseDump(data);
		}

		/// <summary>
		/// Executes a POST-request and returns the result as a JSON object.
		/// </summary>
		/// <param name="method">The GameJolt method to invoke.</param>
		/// <param name="parameters">The arguments to pass to GameJolt.</param>
		/// <param name="payload">The payload arguments passed to the POST-request.</param>
		/// <returns></returns>
		internal async Task<Response<JSONNode>> PostAsync([NotNull] string method, IDictionary<string, string> parameters,
			IDictionary<string, string> payload) {
			method.ThrowIfNullOrEmpty();
			var response = await http.PostAsync(GetRequestUrl(method, parameters, payload, ResponseFormat.Json),
				new FormUrlEncodedContent(payload ?? Enumerable.Empty<KeyValuePair<string, string>>()));
			var data = await response.Content.ReadAsStringAsync();
			return ParseJson(data);
		}

		/// <summary>
		/// Executes a POST-request and returns the result as a raw string.
		/// </summary>
		/// <param name="method">The GameJolt method to invoke.</param>
		/// <param name="parameters">The arguments to pass to GameJolt.</param>
		/// <param name="payload">The payload arguments passed to the POST-request.</param>
		/// <returns></returns>
		internal async Task<Response<string>> PostDumpAsync([NotNull] string method, IDictionary<string, string> parameters,
			IDictionary<string, string> payload) {
			method.ThrowIfNullOrEmpty();
			var response = await http.PostAsync(GetRequestUrl(method, parameters, payload, ResponseFormat.Dump),
				new FormUrlEncodedContent(payload ?? Enumerable.Empty<KeyValuePair<string, string>>()));
			var data = await response.Content.ReadAsStringAsync();
			return ParseDump(data);
		}
		#endregion

		#region Url Building
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
		#endregion
	}
}
