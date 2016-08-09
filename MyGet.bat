dotnet pack src/GladNet.Common/ -c Release
dotnet pack src/GladNet.Encryption/ -c Release
dotnet pack src/GladNet.Engine.Common/ -c Release
dotnet pack src/GladNet.Engine.Server/ -c Release
dotnet pack src/GladNet.Message/ -c Release
dotnet pack src/GladNet.Message.Handlers/ -c Release
dotnet pack src/GladNet.Payload/ -c Release
dotnet pack src/GladNet.Serializer/ -c Release

cd ./src/GladNet.Payload.Authentication
dotnet restore
cd ..
cd ..

dotnet pack src/GladNet.Payload.Authentication/ -c Release