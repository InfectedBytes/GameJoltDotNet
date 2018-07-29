using System.Threading.Tasks;
using GameJolt.Objects;
using NUnit.Framework;

namespace GameJolt.UnitTests {
	public sealed class SessionTests : TestBase {
		[Test]
		public async Task OpenClose() {
			var result = await Api.Sessions.OpenAsync(Credentials);
			Assert.That(result.Success);
			result = await Api.Sessions.CloseAsync(Credentials);
			Assert.That(result.Success);
		}

		[Test]
		public async Task Check() {
			var result = await Api.Sessions.CheckAsync(Credentials);
			Assert.That(!result.Success); // no session open => success == false && no error message
			Assert.That(result.Message == "");

			Assert.That((await Api.Sessions.OpenAsync(Credentials)).Success); // open session
			Assert.That((await Api.Sessions.CheckAsync(Credentials)).Success); // check session => OK
			Assert.That((await Api.Sessions.CloseAsync(Credentials)).Success); // close session

			Assert.That(!(await Api.Sessions.CheckAsync(Credentials)).Success); // check session => no session
		}

		[Test]
		public async Task CheckError() {
			var wrongCredentials = new Credentials(Credentials.Name, "1234");
			var result = await Api.Sessions.CheckAsync(wrongCredentials);
			Assert.That(!result.Success);
			Assert.That(!string.IsNullOrEmpty(result.Message));
		}

		[Test]
		public async Task Ping() {
			Assert.That((await Api.Sessions.OpenAsync(Credentials)).Success); // open session
			Assert.That((await Api.Sessions.PingAsync(Credentials, true)).Success); // ping
			Assert.That((await Api.Sessions.CloseAsync(Credentials)).Success); // close session
		}

		[Test]
		public async Task PingFail() {
			Assert.That(!(await Api.Sessions.PingAsync(Credentials, true)).Success);
		}
	}
}
