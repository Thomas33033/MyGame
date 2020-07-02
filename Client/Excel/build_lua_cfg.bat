@SET EXCEL_FOLDER=.
@SET PYTHONSCRIPT=./excel2Lua/convertor.py
@SET CONTENT=contents.xlsx
@SET ClientWorkDir=%cd%

del configs\client\*.lua /q /f 
del configs\server\*.lua /q /f 

python ./excel2Lua/convertor.py contents.xlsx  ./configs/client/ 1

python ./excel2Lua/convertor.py contents.xlsx  ./configs/server/ 0

xcopy  configs\client ..\..\..\Client\trunk\GameProject\Assets\Lua\Logic\Configs\ /Y
xcopy  configs\client ..\..\..\Client\trunk\BattleServer\WorkSpace\Resource\Datas\Tables\ /Y
pause