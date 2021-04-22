@SET EXCEL_FOLDER=.
@SET PYTHONSCRIPT=./excel2Lua/convertor.py
@SET CONTENT=contents.xlsx
@SET ClientWorkDir=%cd%

del configs\json\*.json /q /f

python ./excel2Lua/conver2Json.py contents.xlsx  ./configs/json/
xcopy  configs\json ..\..\Client\Trunk\Assets\BundleRes\Config\data\ /Y
pause