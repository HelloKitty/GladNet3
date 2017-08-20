xbuild ./src/GladNet.Common/GladLive.Common.csproj /p:DebugSymbols=False
xbuild ./src/GladNet.Encryption/GladNet.Encryption.csproj /p:DebugSymbols=False
xbuild ./src/GladNet.Engine.Common/GladNet.Engine.Common.csproj /p:DebugSymbols=False
xbuild ./src/GladNet.Engine.Server/GladNet.Engine.Server.csproj /p:DebugSymbols=False
xbuild ./src/GladNet.Message/GladLive.Message.csproj /p:DebugSymbols=False
xbuild ./src/GladNet.Message.Handlers/GladNet.Message.Handlers.csproj /p:DebugSymbols=False
xbuild ./src/GladNet.Payload/GladLive.Payload.csproj /p:DebugSymbols=False
xbuild ./src/GladNet.Serializer/GladNet.Serializer.csproj /p:DebugSymbols=False
xbuild ./tests/GladNet.Serializer.Tests/GladNet.Serializer.Tests.csproj /p:DebugSymbols=False

xbuild ./tests/GladNet.Engine.Common.Tests/GladNet.Engine.Common.Tests.csproj /p:DebugSymbols=False
xbuild ./tests/GladNet.Engine.Server.Tests/GladNet.Engine.Server.Tests.csproj /p:DebugSymbols=False
xbuild ./tests/GladNet.Message.Tests/GladLive.Message.Tests.csproj /p:DebugSymbols=False
xbuild ./tests/GladNet.Message.Handlers.Tests/GladNet.Message.Handlers.Tests.csproj /p:DebugSymbols=False
xbuild ./tests/GladNet.Serializer.Tests/GladNet.Serializer.Tests.csproj /p:DebugSymbols=False