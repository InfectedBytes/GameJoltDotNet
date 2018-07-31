using GameJolt.Utils;

namespace GameJolt.Objects {
	/// <summary>
	/// Defines a single scoreboard entry.
	/// </summary>
	public sealed class Score {
		/// <summary>
		/// The score's numerical sort value, for e.g. 100
		/// </summary>
		public int Value { get; }

		/// <summary>
		/// The score string, for e.g.: 100 Coins
		/// </summary>
		public string Text { get; }

		/// <summary>
		/// Any extra data associated with the score, for e.g.: Level 2
		/// </summary>
		public string Extra { get; }

		/// <summary>
		/// If this is a user score, this is the display name for the user. 
		/// </summary>
		public string UserName { get; }

		/// <summary>
		/// If this is a user score, this is the user's ID. 
		/// </summary>
		public int UserId { get; }

		/// <summary>
		/// If this is a guest score, this is the guest's submitted name. 
		/// </summary>
		public string GuestName { get; }

		/// <summary>
		/// If this is a user score, this is the display name for the user.
		/// If this is a guest score, this is the guest's submitted name. 
		/// </summary>
		public string Name => IsGuestScore ? GuestName : UserName;

		/// <summary>
		/// Whether this score was achieved by a guest or not.
		/// </summary>
		public bool IsGuestScore => !string.IsNullOrEmpty(GuestName);

		/// <summary>
		/// Returns when the score was logged by the user, for e.g.: 1 week ago
		/// </summary>
		public string Stored { get; }

		/// <summary>
		/// int	Returns the timestamp (in seconds) of when the score was logged by the user. 
		/// </summary>
		public int StoredTimestamp { get; }

		internal Score(JSONNode data) {
			Value = data["sort"].AsInt;
			Text = data["score"].Value;
			Extra = data["extra_data"].Value;
			UserName = data["user"].Value;
			UserId = data["user_id"].AsInt;
			GuestName = data["guest"].Value;
			Stored = data["stored"].Value;
			StoredTimestamp = data["stored_timestamp"].AsInt;
		}
	}
}
