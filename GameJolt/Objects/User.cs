using System;
using GameJolt.Utils;

namespace GameJolt.Objects {
	/// <summary>
	/// User types.
	/// </summary>
	public enum UserType {
		Undefined,
		User,
		Developer,
		Moderator,
		Admin
	}

	/// <summary>
	/// User statuses.
	/// </summary>
	public enum UserStatus {
		Undefined,
		Active,
		Banned
	}

	public sealed class User {
		/// <summary>
		/// The ID of the user. 
		/// </summary>
		public int Id { get; }

		/// <summary>
		/// The user's username. 
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// The type of user. Can be User, Developer, Moderator, or Administrator. 
		/// </summary>
		public UserType Type { get; }

		/// <summary>
		/// The URL of the user's avatar. 
		/// </summary>
		public string AvatarUrl { get; }

		/// <summary>
		/// How long ago the user signed up. 
		/// </summary>
		public string SignedUp { get; }

		/// <summary>
		/// The timestamp (in seconds) of when the user signed up. 
		/// </summary>
		public int SignedUpTimestamp { get; }

		/// <summary>
		/// How long ago the user was last logged in. Will be Online Now if the user is currently online. 
		/// </summary>
		public string LastLoggedIn { get; }

		/// <summary>
		/// The timestamp (in seconds) of when the user was last logged in. 
		/// </summary>
		public int LastLoggedInTimestamp { get; }

		/// <summary>
		/// Active if the user is still a member of the site. Banned if they've been banned. 
		/// </summary>
		public UserStatus Status { get; }

		/// <summary>
		/// The user's display name.
		/// </summary>
		public string DeveloperName { get; }

		/// <summary>
		/// The user's website (or empty string if not specified) 
		/// </summary>
		public string DeveloperWebsite { get; }

		/// <summary>
		/// The user's profile description. HTML tags and line breaks will be removed. 
		/// </summary>
		public string DeveloperDescription { get; }

		internal User(JSONNode data) {
			Name = data["username"].Value;
			Id = data["id"].AsInt;
			AvatarUrl = data["avatar_url"].Value;
			SignedUp = data["signed_up"].Value;
			SignedUpTimestamp = data["signed_up_timestamp"].AsInt;
			LastLoggedIn = data["last_logged_in"].Value;
			LastLoggedInTimestamp = data["last_logged_in_timestamp"].AsInt;
			DeveloperName = data["developer_name"].Value;
			DeveloperWebsite = data["developer_website"].Value;
			DeveloperDescription = data["developer_description"].Value;

			try {
				Type = (UserType)Enum.Parse(typeof(UserType), data["type"].Value);
			} catch {
				Type = UserType.Undefined;
			}

			try {
				Status = (UserStatus)Enum.Parse(typeof(UserStatus), data["status"].Value);
			} catch {
				Status = UserStatus.Undefined;
			}
		}

		public override string ToString() {
			return $"{Name} ({Id})";
		}
	}
}
