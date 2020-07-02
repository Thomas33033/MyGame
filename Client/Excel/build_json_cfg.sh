CONTENT=contents.xlsx
ClientWorkDir=$(pwd)

echo $CONTENT

echo $ClientWorkDir

rm -r ./configs/csharp/*.cs

python ./excel2Lua/conver2cs.py contents.xlsx  ./configs/csharp/

rm -r ../../../Client/trunk/GameProject/Assets/Scripts/BattleFight/TableData/cfg_*.cs

cp ./configs/csharp/*.cs ../../../Client/trunk/GameProject/Assets/Scripts/BattleFight/TableData