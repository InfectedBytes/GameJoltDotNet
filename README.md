[Documentation](https://infectedbytes.github.io/GameJoltDotNet/) | [GameJolt Page](https://gamejolt.com/dashboard/games/358157) | [NuGet Package](https://www.nuget.org/packages/GameJoltDotNet/) | [License](https://github.com/InfectedBytes/GameJoltDotNet/blob/master/LICENSE)

# GameJoltDotNet
GameJolt [Game API](https://gamejolt.com/game-api/doc) client for the .NET Framework, written entirely in C#.

For best portability this library is based on .NET Standard 1.3 and can therefore be used on a wide variety of different platforms. 
The following platforms are supported:

Framework | Version
----- | -----
.NET Standard | 1.3
.NET Core | 1.0
.NET Framework | 4.6
Mono | 4.6
Xamarin.iOS | 10.0
Xamarin.Mac | 3.0
Xamarin.Android | 7.0
Universal Windows Platform | 10.0

It can even be used with Unity, by changing Unity's [runtime scripting](https://docs.unity3d.com/Manual/ScriptingRuntimeUpgrade.html) environment.

*If you're looking for a pure Unity version, have a look at my [GameJolt Unity API](https://github.com/InfectedBytes/gj-unity-api), it even has a beautiful UI with a GameJolt style.*

## GameJolt API Overview
Service | Description
----- | -----
Datastore | Manipulate items in a cloud-based data storage.
Time | Get the server's time.
Scores | Manipulate scores on score tables.
Sessions | Set up sessions for your game.
Trophies | Manage trophies for your game.
Users | Access user-based features.
Friends | List a user's friends.

This library is heavily based on the async/await mechanism of C# and can therefore be used without blocking the main thread. 
Furthermore it provides a convenience facade API, which internally uses the async/await API, but provides a simple, callback based API as a front end.

## Example
First of all you have to create a new api instance:
```cs
var api = new GameJoltApi(gameId, privateKey);
```
Afterwards you can directly use the different GameJolt features. 
```cs
// by using async/await:
var response = await api.Scores.FetchAsync();
if(response.Success) {
    // response.Data contains the list of scores
}

// or by using the callback mechanism:
api.Scores.Fetch(callback: response => {
    if(response.Success) {
        // response.Data contains the list of scores
    }
});
```

Some APIs require an authenticated user, because they set or get data for a certain user. The authentication can be done like so:
```cs
var auth = await api.Users.AuthAsync(userName, userToken);
if(auth.Success) {
    api.Trophies.SetAchieved(auth.Data, trophyId);
}
```

## Unit Tests
This library is also unit tested. 
When you first run the unit tests, they will immediately fail and create an empty settings file, which must be filled with your games informations.
This settings file can be found in the unit tests binary folder, by default this is `GameJoltDotNet/UnitTests/bin/Debug/settings.json` and looks like this:
```cs
{
	"gameId" : 0, // this is the id of your game
	"privateKey" : "", // this is the private key of your game
	"user" : ["username", "token"], // username and token of the test user
	"secondUser" : ["username", "token"], // optional credentials of another user (only used by a few tests)
	"uniqueTable" : 0, // id of a scoreboard table with unique scores
	"nonUniqueTable" : 0, // id of a scoreboard table without unique scores
	"tables" : [ // array of all tables of your game
		{"id":0, "name":"Main", "description":"", "primary":true},
	],
	"trophies" : [ // array of a few trophies of your game
		{"id":"0","difficulty":"Bronze"},
	]
}
```
**Caution:** The unit tests will execute several api requests to GameJolt, for example they will add and remove datastore entries and they will add user scores.
So if you want to run the unit tests, you should create seaparate scoreboards just for testing.

## Documentation
More documentation and examples will follow, stay tuned!
