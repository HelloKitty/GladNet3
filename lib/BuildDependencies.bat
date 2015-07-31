%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe .\LoggingServices/LoggingServices.sln /p:Configuration=Release /p:Platform="Any CPU"
xcopy  /R /E /Y /q ".\LoggingServices\LoggingServices\bin\Release" ".\Dependency Builds\LoggingServices\DLLs\"

%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe .\Lidgren/Lidgren.Network.sln /p:Configuration=Release /p:Platform="Any CPU"
xcopy  /R /E /Y /q ".\Lidgren\Lidgren.Network\bin\Release" ".\Dependency Builds\Lidgren\DLLs\"
PAUSE
