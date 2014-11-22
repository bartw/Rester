Rester
=
Description
-
Rester is a Windows 8.1 and Windows Phone 8.1 library for talking to restful api's. I'm building Rester instead of using an existing library because I want to learn about restful api's.
Rester is under heavy development and by no means stable. If you are looking for a stable restclient I suggest you use [RestSharp](http://restsharp.org/).
Disclaimer
-
Rester is a hobby project that I do because I love developing. I do however have a life and family. Therefore I can't give any guarantees about deadlines, crashes or any other problems.
Features
-
* GET request
* OAuth1 authentication
* Custom headers
* Request throttling

Problems, questions and suggestions
-
If you have problems, questions or suggestions you can post them in the [issues](https://github.com/bartw/Rester/issues) of this repository.
NuGet
-
You can add Rester to you project using the following command in the package manager console: 
Install-Package Rester.dll, or go to [NuGet.org](https://www.nuget.org/packages/Rester.dll/1.0.0).
Usage
-
```c#
//create a basic Rester client
var client = new Client();
//or create a throttled Rester client with 10 request in every 3 seconds
var client = new ThrottledClient(10, TimeSpan.FromSeconds(3));
//create a Rester get request
var request = new GetRequest("http://api.discogs.com/releases/2817604");
//create an optional authenticator
var authenticator = new OAuth1Authenticator(SignatureMethod.PLAINTEXT, consumerKey, consumerSecret, tokenKey, tokenSecret, verifier);
//send the request and await the response
var response = await client.ExecuteRawAsync(request, authenticator);
```
