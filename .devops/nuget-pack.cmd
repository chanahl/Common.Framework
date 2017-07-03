@ECHO OFF
:: == Pack.cmd ================================================================
:: Runs:
::  nuget pack <.csproj> -Output <output> <suffixOption> -Properties Configuration=<configuration> -Symbols -IncludeReferencedProjects
:: 
:: Input:
::  nupkg = <nupkg>
:: ============================================================================

:: == Input ===================================================================
REM SET nupkg=Common.Framework.Core
REM SET nupkg=Common.Framework.Data
REM SET nupkg=Common.Framework.Network
REM SET nupkg=Common.Framework.Utilities
:: ============================================================================

:: == Setup ===================================================================
SET "base=D:\Git\Common.Framework\"
SET "configuration=%1"
SET "suffixOption="
IF "%configuration%" == "" (
    SET "configuration=Debug"
    SET "suffixOption=-Suffix alpha"
) ELSE (
    SET "configuration=Release"
)

SET "output=%2"
IF "%output%" == "" (
    SET "output=D:\Git\Common.Framework\.nupkgs"
)
MD %output% 2>NUL
:: ============================================================================

:: == Main ====================================================================
CALL :%nupkg%
CALL :Pack %projectPath%
CALL :Finish
:: ============================================================================

:: == nupkg ===================================================================
:Common.Framework.Core
SET projectPath="%base%Common.Framework\Common.Framework.Core\Common.Framework.Core.csproj"
GOTO :EOF

:Common.Framework.Data
SET projectPath="%base%Common.Framework\Common.Framework.Data\Common.Framework.Data.csproj"
GOTO :EOF

:Common.Framework.Network
SET projectPath="%base%Common.Framework\Common.Framework.Network\Common.Framework.Network.csproj"
GOTO :EOF

:Common.Framework.Utilities
SET projectPath="%base%Common.Framework\Common.Framework.Utilities\Common.Framework.Utilities.csproj"
GOTO :EOF
:: ============================================================================

:Pack
SETLOCAL
SET projectPath=%1
nuget pack "%projectPath%" -Output %output% %suffixOption% -Properties Configuration=%configuration% -Symbols -IncludeReferencedProjects
ENDLOCAL
GOTO :EOF

:Finish
PAUSE