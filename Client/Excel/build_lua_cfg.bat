@SET EXCEL_FOLDER=.
@SET PYTHONSCRIPT=./excel2Lua/conver2Lua.py
@SET CONTENT=contents.xlsx
@SET ClientWorkDir=%cd%

del configs\client\*.lua /q /f 
del configs\server\*.lua /q /f 

D:\Python37-32/python ./excel2Lua/conver2Lua.py contents.xlsx  ./configs/client/ 1

D:\Python37-32/python ./excel2Lua/conver2Lua.py contents.xlsx  ./configs/server/ 0

xcopy  configs\client ..\Trunk\Assets\LuaScript\Configs\ /Y
::xcopy  configs\client ..\..\..\Client\trunk\BattleServer\WorkSpace\Resource\Datas\Tables\ /Y
pause