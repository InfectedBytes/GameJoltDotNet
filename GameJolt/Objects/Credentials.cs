namespace GameJolt.Objects {
	public sealed class Credentials {
		public string Name { get; }
		public string Token { get; }

		internal Credentials(string name, string token) {
			Name = name;
			Token = token;
		}
	}
}
