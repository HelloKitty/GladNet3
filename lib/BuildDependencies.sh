xbuild ./LoggingServices/LoggingServices.sln /p:Configuration=Release /p:Platform="Any CPU"
cp -a --remove-destination /LoggingServices/src/LoggingServices/bin/Release/. /Dependency Builds/LoggingServices/DLLs/

xbuild ./Lidgren/Lidgren.Network.sln /p:Configuration=Release /p:Platform="Any CPU"
cp -a --remove-destination /Lidgren/Lidgren.Network/bin/Release/. /Dependency Builds/Lidgren/DLLs/

xbuild ./Net35Essentials/Net35Essentials.sln /p:Configuration=Release /p:Platform="Any CPU"
cp -a --remove-destination /Net35Essentials/src/Net35Essentials/bin/Release/. /Dependency Builds/Net35Essentials/DLLs/
