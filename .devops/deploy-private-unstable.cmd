@ECHO OFF
:: Nexus Repository OSS | deploy
@SET ApiKey=eec72a1a-d9ef-32e8-9254-69dc422dc841
@SET Source=http://desktop-nns09r8:8081/repository/nuget-private-develop/
REM @SET Source=http://desktop-nns09r8:8081/repository/nuget-private-feature/

:: Input
@SET Base=..\.nupkgs\
@SET Version=0.0.0-alpha

@nuget push "%Base%Common.Framework.Core.%Version%.symbols.nupkg" %ApiKey% -Source %Source%
@nuget push "%Base%Common.Framework.Data.%Version%.symbols.nupkg" %ApiKey% -Source %Source%
@nuget push "%Base%Common.Framework.Network.%Version%.symbols.nupkg" %ApiKey% -Source %Source%
@nuget push "%Base%Common.Framework.Utilities.%Version%.symbols.nupkg" %ApiKey% -Source %Source%

PAUSE