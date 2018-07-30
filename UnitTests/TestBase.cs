using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using GameJolt.Objects;
using GameJolt.Utils;
using NUnit.Framework;

namespace GameJolt.UnitTests {
	public class TestBase {
		protected GameJoltApi Api { get; private set; }
		protected Credentials Credentials { get; private set; }
		protected Credentials SecondCredentials { get; private set; }
		protected IReadOnlyCollection<Table> Tables { get; private set; }
		protected int UniqueTable { get; private set; }
		protected int NonUniqueTable { get; private set; }
		protected IReadOnlyCollection<Trophy> Trophies { get; private set; }

		[OneTimeSetUp]
		public void Setup() {
			var file = Path.Combine(TestContext.CurrentContext.TestDirectory, "settings.json");
			if(!File.Exists(file)) {
				File.WriteAllText(file,
@"{
	""gameId"" : 0,
	""privateKey"" : null,
	""user"" : [""<username>"", ""<token>""],
	""secondUser"" : [""<username>"", ""<token>""],
	""uniqueTable"" : 0,
	""nonUniqueTable"" : 0,
	""tables"" : [0, 0],
	""trophies"" : [
		{""id"":""0"",""difficulty"":""Bronze""},
		{""id"":""0"",""difficulty"":""Silver""},
	]
}
			");
			}
			var json = JSONNode.Parse(File.ReadAllText(file));
			var id = json["gameId"].AsInt;
			var key = json["privateKey"].Value;
			Credentials = GetCredentials(json["user"]);
			SecondCredentials = GetCredentials(json["secondUser"]);
			UniqueTable = json["uniqueTable"].AsInt;
			NonUniqueTable = json["nonUniqueTable"].AsInt;
			Tables = json["tables"].ArraySelect(x => new Table(x));
			Trophies = json["trophies"].ArraySelect(x => new Trophy(x));

			Assert.That(id != 0, "Invalid game id");
			Assert.That(!string.IsNullOrEmpty(key), "Invalid private key");
			Assert.That(UniqueTable != 0, "Invalid test table id");
			Assert.That(NonUniqueTable != 0, "Invalid test table id");
			Assert.That(Tables != null, "Invalid table array");
			Assert.That(Tables.Count >= 2, "Table array must contain at least two table ids");
			Assert.That(Tables.Any(t => t.Id == UniqueTable), "Table array does not contain the test table");
			Assert.That(Tables.Any(t => t.Id == NonUniqueTable), "Table array does not contain the test table");
			Assert.That(Trophies.Count >= 2, "Trophy array must contain at least two trophies");

			Api = new GameJoltApi(id, key);
		}

		[TearDown]
		public void PostTest() {
			// too many requests in a too short time frame might be rejected by GameJolt
			Thread.Sleep(100);
		}

		private Credentials GetCredentials(JSONNode data) {
			var name = data[0].Value;
			Assert.That(!string.IsNullOrEmpty(name));
			var token = data[1].Value;
			Assert.That(!string.IsNullOrEmpty(token));
			return new Credentials(name, token);
		}
	}
}
