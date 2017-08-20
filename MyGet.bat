%NUGET% restore GladNetV2.sln -NoCache -NonInteractive -ConfigFile Nuget.config
msbuild GladNetV2.sln /p:Configuration=Release