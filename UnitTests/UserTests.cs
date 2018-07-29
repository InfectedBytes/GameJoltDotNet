using System.Threading.Tasks;
using GameJolt.Objects;
using NUnit.Framework;

namespace GameJolt.UnitTests {
	public sealed class UserTests : TestBase {
		#region Auth
		[Test]
		public async Task Auth() {
			var result = await Api.Users.AuthAsync(Credentials.Name, Credentials.Token);
			Assert.That(result.Success);
			Assert.That(result.Data.Name == Credentials.Name);
			Assert.That(result.Data.Token == Credentials.Token);
		}

		[Test]
		public async Task AuthFail() {
			var result = await Api.Users.AuthAsync(Credentials.Name, "1234");
			Assert.That(!result.Success);
			Assert.That(result.Data == null);
		}
		#endregion

		#region Fetch
		[Test]
		public async Task FetchByName() {
			var result = await Api.Users.FetchAsync("CROS");
			Assert.That(result.Success);
			Assert.That(result.Data.Id == 1);
			Assert.That(result.Data.Name == "CROS");
			Assert.That(result.Data.Type == UserType.Developer);
			Assert.That(result.Data.SignedUpTimestamp == 1229755204);
		}

		[Test]
		public async Task FetchById() {
			var result = await Api.Users.FetchAsync(1);
			Assert.That(result.Success);
			Assert.That(result.Data.Id == 1);
			Assert.That(result.Data.Name == "CROS");
			Assert.That(result.Data.Type == UserType.Developer);
			Assert.That(result.Data.SignedUpTimestamp == 1229755204);
		}

		[Test]
		public async Task FetchByIds() {
			var ids = new[] {1, 2, 3};
			var names = new[] {"CROS", "Kayin", "SomeKid"};
			var result = await Api.Users.FetchAsync(ids);
			Assert.That(result.Success);
			Assert.That(result.Data.Length == 3);
			for(int i = 0; i < ids.Length; i++) {
				var user = result.Data[i];
				Assert.That(user.Id == ids[i]);
				Assert.That(user.Name == names[i]);
			}
		}

		[Test]
		public async Task FetchFail() {
			var result = await Api.Users.FetchAsync("this_user_does_hopefully_not_exist");
			Assert.That(!result.Success);
			Assert.That(result.Data == null);
		}
		#endregion
	}
}
