# GladNet3 (Glader's Library of Advanced Development for Network Emulation Technologies)

GladNet3 is the 3rd generation of the .NET/C# GladNet networking library. The library evolved overtime to support the niche of server/client emulation for game development. The implementation supports already two major emulation implementations such as [World of Warcraft](https://www.github.com/FreecraftCore) and [Phantasy Star Online](https://www.github.com/HelloKitty/Booma.Proxy). In short GladNet3 is a library that contains a collection of APIs that allow you to quickly assemble a client that can connect to game servers. It has extendable support for multiple cryptography schemes and serializers. The recommended serialization for custom binary protocols is the [FreecraftCore.Serializer](https://www.github.com/FreecraftCore) which already supports Phantasy Star Online, World of Warcraft and many other custom binary protocols.

## Why

Emulation development for networking has not had any unification. If you want to create a network client or server to emulate a game there aren't very many useful emulation specific libraries to get up and running quickly. GladNet3 fills a niche in providing the components and APIs required to get something from working very quickly. Combined with the highly configurable [FreecraftCore.Serializer](https://www.github.com/FreecraftCore) you can begin to build and send binary DTOs with the GladNet3 network client on day one. 

The high productivity and the unified approach allow the emulation community to collaberate and focus efforts which support other unrelated projects. For example, preformance improvements merged from efforts by project **A** would also benefit unrelated project **B** and would in turn affect several downstream emulation projects that depend on it.

Just as ASP.NET has provided a common framework for .NET web development GladNet3 tries to do the same. Adopting a similar Handler (controller) API with provided contexts that allow you to act on the message. A unified framework for emulation client/server development is just as benficial as the Microsoft ASP.NET project has been for other .NET developers.

## Features

[x] Async
[x] .NET TcpClient
[] HTTP Client
[] (R)UDP Client
[x] [FreecraftCore.Serializer](https://www.github.com/FreecraftCore) (Attribute-based custom binary protocol serializer)
[] JSON
[] Protobuf
[] XML

## Builds

MyGet: TODO
NuGet: TODO

## Tests

TODO

## License

Contributions including pull requests, commits, notes, dumps, gists or anything else in the repository are licensed under the below licensing terms.

AGPL with the exception that an unrestricted perpetual non-exclusive license is granted to [HelloKitty](www.github.com/HelloKitty) (Andrew Blakely)
