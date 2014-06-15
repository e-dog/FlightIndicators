@echo off
set KSP=C:\games\KSPtest
%WINDIR%\Microsoft.NET\Framework64\v3.5\csc /nologo /t:library^
 /out:"%KSP%\GameData\FlightIndicators\FlightIndicators.dll"^
 /r:"%KSP%\KSP_Data\Managed\Assembly-CSharp.dll"^
 /r:"%KSP%\KSP_Data\Managed\UnityEngine.dll"^
 gui.cs parser.cs wrapper.cs ast.cs
if errorlevel 1 goto exit
echo ======================================
:exit
