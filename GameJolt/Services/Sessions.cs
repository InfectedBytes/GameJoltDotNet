using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameJolt.Objects;
using GameJolt.Utils;
using JetBrains.Annotations;

namespace GameJolt.Services {
	/// <summary>
	/// Sessions are used to tell Game Jolt when a user is playing a game, 
	/// and what state they are in while playing (active or idle).
	/// </summary>
	public sealed class Sessions : Service {
		internal Sessions([NotNull] GameJoltApi api) : base(api) { }

		#region Task Api
		/// <summary>
		/// Opens a game session for a particular user and allows you to tell Game Jolt that a user is playing your game. 
		/// You must ping the session to keep it active and you must close it when you're done with it.
		/// </summary>
		/// <param name="credentials">The user's credentials for whom a session should be opened.</param>
		/// <returns></returns>
		public async Task<Response> OpenAsync([NotNull] Credentials credentials) {
			credentials.ThrowIfNull();
			return await Api.GetAsync("/sessions/open", new Dictionary<string, string> {
				{"username", credentials.Name},
				{"user_token", credentials.Token}
			});
		}

		/// <summary>
		/// Pings an open session to tell the system that it's still active. 
		/// If the session hasn't been pinged within 120 seconds, the system will close the session 
		/// and you will have to open another one. 
		/// It's recommended that you ping about every 30 seconds or so to keep the system from clearing out your session.
		/// You can also let the system know whether the player is in an active or idle state within your game.
		/// </summary>
		/// <param name="credentials">The user's credentials.</param>
		/// <param name="active">Whether the user is active or not.</param>
		/// <returns></returns>
		public async Task<Response> PingAsync([NotNull] Credentials credentials, bool active) {
			credentials.ThrowIfNull();
			return await Api.GetAsync("/sessions/ping", new Dictionary<string, string> {
				{"username", credentials.Name},
				{"user_token", credentials.Token},
				{"status", active ? "active" : "idle"}
			});
		}

		/// <summary>
		/// Checks to see if there is an open session for the user. 
		/// Can be used to see if a particular user account is active in the game.
		/// <para>
		/// Caution: GameJolt does not distinguish between a failed request due to an error 
		/// and a request that doesn't succeed because there is no open session.
		/// You can indirectly check this by looking at the message field.
		/// If Success is false and a message is set, an error has occured.
		/// If Success is false and no message is set, the query has succeeded, but there just is no open session.
		/// </para>
		/// </summary>
		/// <param name="credentials">The user's credentials.</param>
		/// <returns></returns>
		public async Task<Response> CheckAsync([NotNull] Credentials credentials) {
			credentials.ThrowIfNull();
			return await Api.GetAsync("/sessions/check", new Dictionary<string, string> {
				{"username", credentials.Name},
				{"user_token", credentials.Token}
			});
		}

		/// <summary>
		/// Closes the active session.
		/// </summary>
		/// <param name="credentials">The user's credentials.</param>
		/// <returns></returns>
		public async Task<Response> CloseAsync([NotNull] Credentials credentials) {
			credentials.ThrowIfNull();
			return await Api.GetAsync("/sessions/close", new Dictionary<string, string> {
				{"username", credentials.Name},
				{"user_token", credentials.Token}
			});
		}
		#endregion

		#region Callback Api
		/// <summary>
		/// Opens a game session for a particular user and allows you to tell Game Jolt that a user is playing your game. 
		/// You must ping the session to keep it active and you must close it when you're done with it.
		/// </summary>
		/// <param name="credentials">The user's credentials for whom a session should be opened.</param>
		/// <param name="callback">Action that is called on completion or error.</param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public void Open([NotNull] Credentials credentials, Action<Response> callback) {
			Wrap(OpenAsync(credentials), callback);
		}

		/// <summary>
		/// Pings an open session to tell the system that it's still active. 
		/// If the session hasn't been pinged within 120 seconds, the system will close the session 
		/// and you will have to open another one. 
		/// It's recommended that you ping about every 30 seconds or so to keep the system from clearing out your session.
		/// You can also let the system know whether the player is in an active or idle state within your game.
		/// </summary>
		/// <param name="credentials">The user's credentials.</param>
		/// <param name="active">Whether the user is active or not.</param>
		/// <param name="callback">Action that is called on completion or error.</param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public void Ping([NotNull] Credentials credentials, bool active, Action<Response> callback) {
			Wrap(PingAsync(credentials, active), callback);
		}

		/// <summary>
		/// Checks to see if there is an open session for the user. 
		/// Can be used to see if a particular user account is active in the game.
		/// <para>
		/// Caution: GameJolt does not distinguish between a failed request due to an error 
		/// and a request that doesn't succeed because there is no open session.
		/// You can indirectly check this by looking at the message field.
		/// If Success is false and a message is set, an error has occured.
		/// If Success is false and no message is set, the query has succeeded, but there just is no open session.
		/// </para>
		/// </summary>
		/// <param name="credentials">The user's credentials.</param>
		/// <param name="callback">Action that is called on completion or error.</param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public void Check([NotNull] Credentials credentials, [NotNull] Action<Response> callback) {
			Wrap(CheckAsync(credentials), callback);
		}

		/// <summary>
		/// Closes the active session.
		/// </summary>
		/// <param name="credentials">The user's credentials.</param>
		/// <param name="callback">Action that is called on completion or error.</param>
		/// <returns></returns>
		[ExcludeFromCodeCoverage]
		public void Close([NotNull] Credentials credentials, Action<Response> callback) {
			Wrap(CloseAsync(credentials), callback);
		}
		#endregion
	}
}
