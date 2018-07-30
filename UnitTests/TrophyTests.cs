using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameJolt.Objects;
using GameJolt.Utils;
using NUnit.Framework;

namespace GameJolt.UnitTests {
	public sealed class TrophyTests : TestBase {
		private readonly PartialTrophyComparer trophyComparer = new PartialTrophyComparer();

		#region Fetch
		[Test]
		public async Task Fetch() {
			var result = await Api.Trophies.FetchAsync(Credentials);
			Assert.That(result.Success);
			Assert.That(result.Data.Intersect(Trophies, trophyComparer).Count() == Trophies.Count);
		}

		[TestCase(true)]
		[TestCase(false)]
		public async Task FetchAchieved(bool achieved) {
			var id = Trophies.First().Id;
			Assert.That((await Api.Trophies.SetAchievedAsync(Credentials, id)).Success);
			var trophies = await GetTrophies(achieved);
			Assert.That(trophies.All(x => x.Achieved == achieved));
			Assert.That((await Api.Trophies.RemoveAchievedAsync(Credentials, id)).Success);
		}

		[Test]
		public async Task FetchIds() {
			var result = await Api.Trophies.FetchAsync(Credentials, ids: Trophies.Select(x => x.Id).ToArray());
			Assert.That(result.Success);
			Assert.That(result.Data.Intersect(Trophies, trophyComparer).SequenceEqual(Trophies, trophyComparer));
		}
		#endregion

		#region Add/Remove
		[Test]
		public async Task AddRemove() {
			var id = Trophies.First().Id;
			// add trophy
			Assert.That((await Api.Trophies.SetAchievedAsync(Credentials, id)).Success);
			var trophies = await GetTrophies();
			Assert.That(trophies.Any(x => x.Achieved && x.Id == id));
			// remove trophy
			Assert.That((await Api.Trophies.RemoveAchievedAsync(Credentials, id)).Success);
			trophies = await GetTrophies();
			Assert.That(!trophies.Any(x => x.Achieved && x.Id == id));
		}
		#endregion

		#region Utils
		private async Task<Trophy[]> GetTrophies(bool? achieved = null, int[] ids = null) {
			var result = await Api.Trophies.FetchAsync(Credentials, achieved, ids);
			Assert.That(result.Success);
			return result.Data;
		}
		#endregion

		#region Comparer
		[ExcludeFromCodeCoverage]
		private sealed class PartialTrophyComparer : IEqualityComparer<Trophy> {
			public bool Equals(Trophy x, Trophy y) {
				if(x == y) return true;
				if(x == null | y == null) return false;
				if(x.Id != y.Id) return false;
				return x.Difficulty == y.Difficulty;
			}
			
			public int GetHashCode(Trophy obj) {
				return obj.Id * obj.Difficulty.GetHashCode();
			}
		}
		#endregion
	}
}
