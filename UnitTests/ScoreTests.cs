using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameJolt.Objects;
using GameJolt.Utils;
using NUnit.Framework;

namespace GameJolt.UnitTests {
	public sealed class ScoreTests : TestBase {
		private readonly PartialTableComparer tableComparer = new PartialTableComparer();
		private readonly PartialScoreComparer scoreComparer = new PartialScoreComparer();

		#region Setup
		[OneTimeSetUp]
		public async Task SetupTables() {
			await FillTable(NonUniqueTable);
		}

		[ExcludeFromCodeCoverage]
		private async Task FillTable(int id) {
			var result = await GetScoreValues(id, limit: 15);
			var offset = result.FirstOrDefault() + 1;
			for(int i = result.Length; i < 15; i++) {
				await Add(id, Credentials, i + offset);
			}
		}
		#endregion

		#region Fetch
		[Test]
		public async Task FetchDefault() {
			var defaultResults = await Api.Scores.FetchAsync();
			Assert.That(defaultResults.Success);
			var explicitResults = await Api.Scores.FetchAsync(tableId: Tables.First().Id);
			Assert.That(explicitResults.Success);
			Assert.That(defaultResults.Data.SequenceEqual(explicitResults.Data, scoreComparer));
		}

		[Test]
		public async Task FetchLimits() {
			const int limit = 100;
			var results = await Api.Scores.FetchAsync(limit: limit);
			Assert.That(results.Success);
			Assert.That(results.Data.Length <= limit);
		}

		[Test]
		public async Task FetchGuest() {
			const string guest = "TestGuest";
			var scores = await GetScores(UniqueTable);
			var score = scores.First().Value + 1;
			Assert.That((await Api.Scores.AddAsync(guest, score, $"{score} Points", tableId: UniqueTable)).Success);
			var guestScores = await Api.Scores.FetchAsync(guest, UniqueTable);
			Assert.That(guestScores.Data.Length == 1);
			Assert.That(guestScores.Data[0].Value == score);
		}
		#endregion

		#region Better/Worse than
		[Test]
		public async Task BetterThan() {
			const int betterThanCount = 2;
			var scores = await GetScoreValues(NonUniqueTable);
			var betterThan = scores.Skip(betterThanCount).First(); // reference value
			var betterThanScores = await GetScoreValues(NonUniqueTable, betterThan: betterThan);
			Assert.That(betterThanScores.SequenceEqual(scores.Take(betterThanCount)));
		}

		[Test]
		public async Task WorseThan() {
			const int worseThanCount = 2;
			var scores = await GetScoreValues(NonUniqueTable, limit: 7);
			var worseThan = scores.Skip(worseThanCount).First(); // reference value
			var worseThanScores = await GetScoreValues(NonUniqueTable, limit: 4, worseThan:worseThan);
			Assert.That(worseThanScores.SequenceEqual(scores.Skip(worseThanCount + 1)));
		}

		[Test]
		public async Task BetterWorse() {
			bool thrown = false;
			try {
				await Api.Scores.FetchAsync(betterThan: 10, worseThan: 100);
			} catch(ArgumentException) {
				thrown = true;
			}
			Assert.That(thrown);
		}
		#endregion

		#region Add
		[Test]
		public async Task AddNonUnique() {
			var scores = await GetScoreValues(NonUniqueTable);
			var scoreA = scores.First() + 10;
			var scoreB = scores.First() + 5;
			await Add(NonUniqueTable, Credentials, scoreA);
			await Add(NonUniqueTable, Credentials, scoreB);
			var newScores = await GetScoreValues(NonUniqueTable);
			var expected = new[] { scoreA, scoreB }.Concat(scores).Take(10);
			Assert.That(expected.SequenceEqual(newScores));
		}

		[Test]
		public async Task AddUnique() {
			var score = (await GetScoreValues(UniqueTable, Credentials)).FirstOrDefault() + 1;
			// add a new higher score
			await Add(UniqueTable, Credentials, score);
			var newScores = await GetScoreValues(UniqueTable, Credentials);
			Assert.That(newScores.Length == 1);
			Assert.That(newScores[0] == score);
			// add another, higher score
			await Add(UniqueTable, Credentials, score + 1);
			newScores = await GetScoreValues(UniqueTable, Credentials);
			Assert.That(newScores.Length == 1);
			Assert.That(newScores[0] == score + 1);
			// add a lower score
			await Add(UniqueTable, Credentials, score - 1);
			newScores = await GetScoreValues(UniqueTable, Credentials);
			Assert.That(newScores.Length == 1);
			Assert.That(newScores[0] == score + 1); // score does not change
		}

		[Test]
		public async Task TestMultiUser() {
			await CheckSecondUser();
			var scores = await GetScores(UniqueTable);
			var score = scores.First().Value + 1;
			await Add(UniqueTable, Credentials, score);
			await Add(UniqueTable, SecondCredentials, score + 1);
			await UserOnly(Credentials);
			await UserOnly(SecondCredentials);
			var newScores = await GetScores(UniqueTable);
			Assert.That(newScores.Count(x => x.Name == Credentials.Name) == 1);
			Assert.That(newScores.Count(x => x.Name == SecondCredentials.Name) == 1);

			async Task UserOnly(Credentials credentials) {
				var userOnlyScores = await GetScores(UniqueTable, credentials);
				Assert.That(userOnlyScores.All(x => x.Name == credentials.Name));
			}
		}

		[Test]
		public async Task Guest() {
			const string guest = "TestGuest";
			var scores = await GetScores(UniqueTable);
			var score = scores.First().Value + 1;
			Assert.That((await Api.Scores.AddAsync(guest, score, $"{score} Points", tableId: UniqueTable)).Success);
			Assert.That((await GetScores(UniqueTable)).Count(x => x.Name == guest) == 1);
		}
		#endregion

		#region Rank
		[Test]
		public async Task GetRank() {
			var scores = await GetScoreValues(NonUniqueTable);
			var result = await Api.Scores.GetRankAsync(scores[0] + 1, NonUniqueTable);
			Assert.That(result.Success);
			Assert.That(result.Data == 1);
			result = await Api.Scores.GetRankAsync((scores[0] + scores[1]) / 2, NonUniqueTable);
			Assert.That(result.Success);
			Assert.That(result.Data == 2);
		}
		#endregion

		#region GetTables
		[Test]
		public async Task GetTables() {
			var result = await Api.Scores.FetchTablesAsync();
			Assert.That(result.Success);
			var unexpectedTables = result.Data.Except(Tables, tableComparer).ToArray();
			Assert.That(unexpectedTables.Length == 0, $"Unexpected tables: {string.Join(", ", unexpectedTables.Select(t => t.Name))}");
			var missingTables = Tables.Except(result.Data, tableComparer).ToArray();
			Assert.That(missingTables.Length == 0, $"Missing tables: {string.Join(", ", missingTables.Select(t => t.Name))}");
		}
		#endregion

		#region Utils
		private async Task CheckSecondUser() {
			Assert.That((await Api.Users.AuthAsync(SecondCredentials.Name, SecondCredentials.Token)).Success,
				"In order to run this test, you need a valid second user");
			Assert.That(Credentials.Name != SecondCredentials.Name,
				"The second user must be different from the first one.");
		}

		private async Task Add(int tableId, Credentials credentials, int value) {
			Assert.That((await Api.Scores.AddAsync(credentials, value, $"{value} Points", tableId: tableId)).Success);
		}

		private async Task<Score[]> GetScores(int tableId, Credentials credentials = null, int limit = 10,
			int? betterThan = null, int? worseThan = null) {
			var result = await Api.Scores.FetchAsync(credentials, tableId, limit, betterThan, worseThan);
			Assert.That(result.Success);
			return result.Data;
		}

		private async Task<int[]> GetScoreValues(int tableId, Credentials credentials = null, int limit = 10,
			int? betterThan = null, int? worseThan = null) {
			return (await GetScores(tableId, credentials, limit, betterThan, worseThan)).Select(x => x.Value).ToArray();
		}
		#endregion

		#region Comparer
		/// <summary>
		/// Comparer that only compares the id, name and primary attribute of a table.
		/// </summary>
		[ExcludeFromCodeCoverage]
		private sealed class PartialTableComparer : IEqualityComparer<Table> {
			public bool Equals(Table x, Table y) {
				if(x == y) return true;
				if(x == null || y == null) return false;
				if(x.Id != y.Id) return false;
				if(x.Name != y.Name) return false;
				return x.Primary == y.Primary;
			}

			public int GetHashCode(Table obj) {
				return obj.Id * obj.Name.GetHashCode() + obj.Primary.GetHashCode();
			}
		}

		/// <summary>
		/// Comparer that only compares the timestamp, user id, value and name attributes.
		/// </summary>
		[ExcludeFromCodeCoverage]
		private sealed class PartialScoreComparer : IEqualityComparer<Score> {
			public bool Equals(Score x, Score y) {
				if(x == y) return true;
				if(x == null | y == null) return false;
				if(x.StoredTimestamp != y.StoredTimestamp) return false;
				if(x.UserId != y.UserId) return false;
				if(x.Value != y.Value) return false;
				return x.Name == y.Name;
			}

			public int GetHashCode(Score obj) {
				return obj.StoredTimestamp * obj.UserId * obj.Value;
			}
		}
		#endregion
	}
}
