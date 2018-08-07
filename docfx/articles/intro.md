# Introduction to `GameJoltDotNet`
The [`GameJolt Game API`](https://gamejolt.com/game-api/doc) is a service provided by [GameJolt](https://gamejolt.com/), 
which provides you with a number of cloud based features for your game, including:
* Online score tables
* Trophies (achievements)
* Cloud data storage
* Game session logging
* User verification

In order to use these features, one has to communicate with the GameJolt servers through their RESTful web service. 

`GameJoltDotNet` is a `.NET` library which implements all of GameJolt's services and provides you with a clean and 
simple interface to communicate with the GameJolt servers. It is written in `C#` and fully documented by using `C#'s` 
xml documentations comments.

## Install
The easiest way to add this library to your project is by using NuGet.
Just add the [`GameJoltDotNet`](https://www.nuget.org/packages/GameJoltDotNet/) NuGet package to your project and you're done.

Alternatively you can also download the compiled dll from [github](https://github.com/InfectedBytes/GameJoltDotNet/releases) 
or [GameJolt](https://gamejolt.com/games/gamejoltdotnet/358157) and then add a reference to it in your project.

## Supported .NET Frameworks
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

## Supported Languages 
Since this library is written in plain `C#`, the compiled DLL can be used by any `.NET` language, for example:
* `C#`
* `Visual Basic .NET`
* `F#`
* `Nemerle`
* etc. 

Just add a reference to `GameJolt.dll` and you're ready to got.

## Simple Example
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
