@echo off
SET "ProjectName=NamesGalore"
SET "SolutionDir=C:\Users\robin\Desktop\Games\RimWorld Modding\Source\NamesGalore\Source"
@echo on

xcopy /S /Y "%SolutionDir%\..\About\*" "D:\SteamLibrary\steamapps\common\RimWorld\Mods\%ProjectName%\About\"
xcopy /S /Y "%SolutionDir%\..\Assemblies\*" "D:\SteamLibrary\steamapps\common\RimWorld\Mods\%ProjectName%\Assemblies\"
xcopy /S /Y "%SolutionDir%\..\Languages\*" "D:\SteamLibrary\steamapps\common\RimWorld\Mods\%ProjectName%\Languages\"