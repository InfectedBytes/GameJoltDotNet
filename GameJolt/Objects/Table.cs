namespace GameJolt.Objects {
	public sealed class Table {
		/// <summary>
		/// The Id of the score table. 
		/// </summary>
		public int Id { get; }

		/// <summary>
		/// The developer-defined name of the score table. 
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// The developer-defined description of the score table. 
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// Whether or not this is the default score table. Scores are submitted to the primary table by default. 
		/// </summary>
		public bool Primary { get; }

		internal Table(JSONNode data) {
			Id = data["id"].AsInt;
			Name = data["name"].Value;
			Description = data["description"].Value;
			Primary = data["primary"].Value != "false" && data["primary"].Value != "0";
		}
	}
}
