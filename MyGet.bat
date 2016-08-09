%MsBuildExe% src/GladNet.Common/GladNet.Common.csproj /p:Configuration=Release
%MsBuildExe% src/GladNet.Encryption/GladNet.Encryption.csproj /p:Configuration=Release
%MsBuildExe% src/GladNet.Engine.Common/GladNet.Engine.Common.csproj /p:Configuration=Release
%MsBuildExe% src/GladNet.Engine.Server/GladNet.Engine.Server.csproj /p:Configuration=Release
%MsBuildExe% src/GladNet.Message/GladNet.Message.csproj /p:Configuration=Release
%MsBuildExe% src/GladNet.Message.Handlers/GladNet.Message.Handlers.csproj /p:Configuration=Release
%MsBuildExe% src/GladNet.Payload/GladNet.Payload.csproj /p:Configuration=Release
%MsBuildExe% src/GladNet.Serializer/GladNet.Serializer.csproj /p:Configuration=Release

PAUSE

cd ./src/GladNet.Payload.Authentication
dotnet restore
cd ..
cd ..

dotnet pack src/GladNet.Payload.Authentication/ -c Release