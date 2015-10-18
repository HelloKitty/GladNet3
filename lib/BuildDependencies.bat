%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe .\LoggingServices/LoggingServices.sln /p:Configuration=Release /p:Platform="Any CPU"
xcopy  /R /E /Y /q ".\LoggingServices\src\LoggingServices\bin\Release" ".\Dependency Builds\LoggingServices\DLLs\"

%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe .\Lidgren/Lidgren.Network.sln /p:Configuration=Release /p:Platform="Any CPU"
xcopy  /R /E /Y /q ".\Lidgren\Lidgren.Network\bin\Release" ".\Dependency Builds\Lidgren\DLLs\"

%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe .\Net35Essentials/Net35Essentials.sln /p:Configuration=Release /p:Platform="Any CPU"
xcopy  /R /E /Y /q ".\Net35Essentials\src\Net35Essentials\bin\Release" ".\Dependency Builds\Net35Essentials\DLLs\"

PAUSE
