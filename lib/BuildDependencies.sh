xbuild ./LoggingServices/LoggingServices.sln /p:Configuration=Release /p:Platform="Any CPU"
mkdir -p Dependency\ Builds/LoggingServices/DLLs/
rsync -avv ./LoggingServices/src/LoggingServices/bin/Release/ Dependency\ Builds/LoggingServices/DLLs/

chmod +x ./GladNet2/lib/BuildDependencies.sh
cd GladNet2/lib/
./BuildDependencies.sh
cd ..
cd ..

nuget restore ./GladNet2/GladNetV2.sln
xbuild ./GladNet2/GladNetV2.sln /p:Configuration=Release /p:Platform="Any CPU"
mkdir -p Dependency\ Builds/GladNet/DLLs/
rsync -avv ./GladNet2/src/GladNet.Common/bin/Release/ Dependency\ Builds/GladNet/DLLs/
rsync -avv ./GladNet2/src/GladNet.Serializer/bin/Release/ Dependency\ Builds/GladNet/DLLs/
rsync -avv ./GladNet2/src/GladNet.Server.Common/bin/Release/ Dependency\ Builds/GladNet/DLLs/

xbuild ./Net35Essentials/Net35Essentials.sln /p:Configuration=Release /p:Platform="Any CPU"
mkdir -p Dependency\ Builds/Net35Essentials/DLLs/
rsync -avv ./Net35Essentials/src/Net35Essentials/bin/Release/ Dependency\ Builds/Net35Essentials/DLLs/