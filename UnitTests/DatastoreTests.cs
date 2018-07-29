using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GameJolt.Services;
using NUnit.Framework;

namespace GameJolt.UnitTests {
	public sealed class DatastoreTests : TestBase {
		private readonly Random rnd = new Random(0);

		#region Setup/Teardown
		[OneTimeSetUp]
		public async Task SetupData() {
			var dataset = GetTestData();
			foreach(var credentials in new[] { null, Credentials })
				foreach(var data in dataset)
					Assert.That((await Api.Datastore.SetAsync(data.Item1, data.Item2, credentials)).Success);
		}

		[OneTimeTearDown]
		public async Task TeardownData() {
			var dataset = GetTestData();
			foreach(var credentials in new[] { null, Credentials })
				foreach(var data in dataset)
					Assert.That((await Api.Datastore.RemoveAsync(data.Item1, credentials)).Success);
		}
		#endregion

		#region Set/Remove
		[TestCase(true)]
		[TestCase(false)]
		public async Task SetAndRemove(bool global) {
			const string key = "__Test__SetAndRemove";
			string data = RandomData();
			var credentials = global ? null : Credentials;
			var result = await Api.Datastore.SetAsync(key, data, credentials);
			Assert.That(result.Success);
			var stored = await Api.Datastore.FetchAsync(key, credentials);
			Assert.That(stored.Success);
			Assert.That(stored.Data == data);
			Assert.That((await Api.Datastore.RemoveAsync(key, credentials)).Success);
			Assert.That(!(await Api.Datastore.FetchAsync(key, credentials)).Success);
		}
		#endregion

		#region GetKeys
		[TestCase(true, "__TestKey__*")]
		[TestCase(true, "__TestKey__0__*")]
		[TestCase(false, "__TestKey__*")]
		[TestCase(false, "__TestKey__0__*")]
		public async Task GetKeys(bool global, string pattern) {
			var credentials = global ? null : Credentials;
			var dataset = GetTestData();
			var regex = new Regex(pattern.Replace("*", ".+"));
			var result = await Api.Datastore.GetKeysAsync(credentials, pattern);
			Assert.That(result.Success);
			var expected = dataset.Select(x => x.Item1).Where(x => regex.IsMatch(x));
			Assert.That(result.Data.SequenceEqual(expected));
		}
		#endregion

		#region Update
		[TestCase("42", DatastoreOperation.Add, "1", "43")]
		[TestCase("42", DatastoreOperation.Subtract, "1", "41")]
		[TestCase("42", DatastoreOperation.Multiply, "2", "84")]
		[TestCase("42", DatastoreOperation.Divide, "2", "21")]
		[TestCase("42", DatastoreOperation.Prepend, "<", "<42")]
		[TestCase("42", DatastoreOperation.Append, ">", "42>")]
		public async Task Operations(string initialValue, DatastoreOperation operation, string updateValue, string expectedValue) {
			const string key = "__Test__Operations";
			Assert.That((await Api.Datastore.SetAsync(key, initialValue)).Success);
			var result = await Api.Datastore.UpdateAsync(key, updateValue, operation);
			Assert.That(result.Success);
			Assert.That(result.Data == expectedValue);
			Assert.That((await Api.Datastore.RemoveAsync(key)).Success);
		}
		#endregion

		#region Utils
		private string Key(int x, int y) { return $"__TestKey__{x}_{y}"; }
		private string Data(int x, int y) { return $"__TestData__{x}_{y}"; }

		private string RandomData() {
			var rndPart = Enumerable.Range(0, 5).Select(x => (char)('a' + rnd.Next(26))).ToArray();
			return $"__TestData__{new string(rndPart)}";
		}

		private IList<Tuple<string, string>> GetTestData() {
			var list = new List<Tuple<string, string>>();
			for(int x = 0; x < 3; x++)
				for(int y = 0; y < 3; y++)
					list.Add(Tuple.Create(Key(x, y), Data(x, y)));
			return list;
		}
		#endregion
	}
}
