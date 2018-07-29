using System.Threading.Tasks;
using NUnit.Framework;

namespace GameJolt.UnitTests {
	public sealed class FriendsTests : TestBase {
		[Test]
		public async Task Fetch() {
			var result = await Api.Friends.FetchAsync(Credentials);
			Assert.That(result.Success);
			foreach(var id in result.Data) {
				var user = await Api.Users.FetchAsync(id);
				Assert.That(user.Success);
				Assert.That(user.Data.Id == id);
			}
		}
	}
}
