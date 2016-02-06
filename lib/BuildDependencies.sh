xbuild ./LoggingServices/LoggingServices.sln /p:Configuration=Release /p:Platform="Any CPU"
mkdir -p Dependency\ Builds/LoggingServices/DLLs/
rsync -avv ./LoggingServices/src/LoggingServices/bin/Release/ Dependency\ Builds/LoggingServices/DLLs/

chmod +x ./GladNet2/lib/BuildDependencies.sh
cd GladNet2/lib/
./BuildDependencies.sh
cd ..
nuget restore GladNetV2.sln
xbuild GladNetV2.sln /p:Configuration=Release /p:Platform="Any CPU"
cd ..

mkdir -p Dependency\ Builds/GladNet/DLLs/
rsync -avv ./GladNet2/src/GladNet.Common/bin/Release/ Dependency\ Builds/GladNet/DLLs/
rsync -avv ./GladNet2/src/GladNet.Serializer/bin/Release/ Dependency\ Builds/GladNet/DLLs/
rsync -avv ./GladNet2/src/GladNet.Server.Common/bin/Release/ Dependency\ Builds/GladNet/DLLs/



xbuild ./Net35Essentials/Net35Essentials.sln /p:Configuration=Release /p:Platform="Any CPU"
mkdir -p Dependency\ Builds/Net35Essentials/DLLs/
rsync -avv ./Net35Essentials/src/Net35Essentials/bin/Release/ Dependency\ Builds/Net35Essentials/DLLs/