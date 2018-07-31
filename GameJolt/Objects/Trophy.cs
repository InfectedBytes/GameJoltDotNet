using System;
using GameJolt.Utils;

namespace GameJolt.Objects {
	public enum TrophyDifficulty {
		Undefined,
		Bronze,
		Silver,
		Gold,
		Platinum
	}

	/// <summary>
	/// Provides the information a trophy.
	/// </summary>
	public sealed class Trophy {
		/// <summary>
		/// The ID of the trophy. 
		/// </summary>
		public int Id { get; }
		
		/// <summary>
		/// The title of the trophy on the site. 
		/// </summary>
		public string Title { get; }

		/// <summary>
		/// The trophy description text. 
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// Bronze, Silver, Gold, or Platinum 
		/// </summary>
		public TrophyDifficulty Difficulty { get; }

		/// <summary>
		/// The URL of the trophy's thumbnail image. 
		/// </summary>
		public string ImageUrl { get; }

		/// <summary>
		/// Date/time when the trophy was achieved by the user, or false if they haven't achieved it yet. 
		/// </summary>
		public bool Achieved { get; }

		/// <summary>
		/// If the user has not yet achieved this trophy, this property determines whether this trophy 
		/// is tagged as a secret trophy. If the user already achieved this trophy, this property returns false.
		/// </summary>
		public bool IsSecret => string.IsNullOrEmpty(Description) && ImageUrl == "https://s.gjcdn.net/img/trophy-secret-1.jpg";

		internal Trophy(JSONNode data) {
			Id = data["id"].AsInt;
			Title = data["title"].Value;
			Description = data["description"].Value;
			ImageUrl = data["image_url"].Value;
			Achieved = data["achieved"].Value != "false";

			try {
				Difficulty = (TrophyDifficulty)Enum.Parse(typeof(TrophyDifficulty), data["difficulty"].Value);
			} catch {
				Difficulty = TrophyDifficulty.Undefined;
			}
		}
	}
}
