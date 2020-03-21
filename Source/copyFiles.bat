@echo off
SET "ProjectName=NamesGalore"
SET "SolutionDir=C:\Users\robin\Desktop\Games\RimWorld Modding\Source\NamesGalore\Source"
SET "RWModsDir=D:\SteamLibrary\steamapps\common\RimWorld\Mods"
@echo on

xcopy /S /Y "%SolutionDir%\..\About\*" "%RWModsDir%\%ProjectName%\About\"
xcopy /S /Y "%SolutionDir%\..\Assemblies\*" "%RWModsDir%\%ProjectName%\Assemblies\"
xcopy /S /Y "%SolutionDir%\..\Languages\*" "%RWModsDir%\%ProjectName%\Languages\"