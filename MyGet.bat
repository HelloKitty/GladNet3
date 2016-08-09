msbuild src/GladNet.Common/GladNet.Common.csproj /p:DebugSymbols=False
msbuild src/GladNet.Encryption/GladNet.Encryption.csproj /p:DebugSymbols=False
msbuild src/GladNet.Engine.Common/GladNet.Engine.Common.csproj /p:DebugSymbols=False
msbuild src/GladNet.Engine.Server/GladNet.Engine.Server.csproj /p:DebugSymbols=False
msbuild src/GladNet.Message/GladNet.Message.csproj /p:DebugSymbols=False
msbuild src/GladNet.Message.Handlers/GladNet.Message.Handlers.csproj /p:DebugSymbols=False
msbuild src/GladNet.Payload/GladNet.Payload.csproj /p:DebugSymbols=False
msbuild src/GladNet.Serializer/GladNet.Serializer.csproj /p:DebugSymbols=False

cd ./src/GladNet.Payload.Authentication
dotnet restore
cd ..
cd ..

dotnet pack src/GladNet.Payload.Authentication/ -c Release