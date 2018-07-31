using GameJolt.Services;

namespace GameJolt.Objects {
	/// <summary>
	/// Stores a user's credentials. 
	/// In order to retrieve an instance of this class, you have to call the 
	/// <see cref="Users.AuthAsync"/>/<see cref="Users.Auth"/> method. 
	/// If that call succeeds, it gives you an instance of this class, which can be used to perform user dependent API calls.
	/// </summary>
	public sealed class Credentials {
		/// <summary>
		/// The user's name.
		/// </summary>
		public string Name { get; }
		/// <summary>
		/// The user's token (NOT his password).
		/// </summary>
		public string Token { get; }

		internal Credentials(string name, string token) {
			Name = name;
			Token = token;
		}
	}
}
