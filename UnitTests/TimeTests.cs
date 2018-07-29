using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace GameJolt.UnitTests {
	public sealed class TimeTests : TestBase {
		[Test]
		public async Task TimeFetch() {
			var result = await Api.Time.GetAsync();
			Assert.That(result.Success);
			Assert.That(result.Data - DateTime.Now < TimeSpan.FromSeconds(2));
		}
	}
}
