call SubmoduleInit.bat < nul

"%ProgramFiles(x86)%\MSBuild\14.0\Bin\msbuild.exe" .\LoggingServices/LoggingServices.sln /p:Configuration=Release /p:Platform="Any CPU"
xcopy  /R /E /Y /q ".\LoggingServices\src\LoggingServices\bin\Release" ".\Dependency Builds\LoggingServices\DLLs\"

"%ProgramFiles(x86)%\MSBuild\14.0\Bin\msbuild.exe" .\Net35Essentials/Net35Essentials.sln /p:Configuration=Release /p:Platform="Any CPU"
xcopy  /R /E /Y /q ".\Net35Essentials\src\Net35Essentials\bin\Release" ".\Dependency Builds\Net35Essentials\DLLs\"

nuget.exe restore GladNet2\GladNetV2.sln
CD .\GladNet2\Lib
call BuildDependencies.bat < nul
CD ..

"%ProgramFiles(x86)%\MSBuild\14.0\Bin\msbuild.exe" GladNetV2.sln /p:Configuration=Release /p:Platform="Any CPU"

CD ..

xcopy  /R /E /Y /q ".\GladNet2\src\GladNet.Common\bin\Release" ".\Dependency Builds\GladNet\DLLs\"
xcopy  /R /E /Y /q ".\GladNet2\src\GladNet.Serializer\bin\Release" ".\Dependency Builds\GladNet\DLLs\"
xcopy  /R /E /Y /q ".\GladNet2\src\GladNet.Server.Common\bin\Release" ".\Dependency Builds\GladNet\DLLs\"

PAUSE