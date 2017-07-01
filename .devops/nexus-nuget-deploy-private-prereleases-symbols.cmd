@ECHO OFF

:: Nexus Repository OSS | deploy
@SET ApiKey=29872fea-8ea4-32c1-95ec-61afbe98a6b7
@SET Symbols=http://desktop-nns09r8:8081/repository/nuget-private-prereleases-symbols/

:: Input
@SET Base=..\.nupkgs\
@SET Version=0.0.0-alpha

:: Command
@nuget push "%Base%Common.Framework.Core.%Version%.symbols.nupkg" %ApiKey% -Source %Symbols%
@nuget push "%Base%Common.Framework.Data.%Version%.symbols.nupkg" %ApiKey% -Source %Symbols%
@nuget push "%Base%Common.Framework.Network.%Version%.symbols.nupkg" %ApiKey% -Source %Symbols%
@nuget push "%Base%Common.Framework.Utilities.%Version%.symbols.nupkg" %ApiKey% -Source %Symbols%

PAUSE