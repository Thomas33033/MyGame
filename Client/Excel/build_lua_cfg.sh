CONTENT=contents.xlsx
ClientWorkDir=$(pwd)

echo $CONTENT

echo $ClientWorkDir

rm -r ./configs/client/*.lua
rm -r ./configs/server/*.lua

python ./excel2Lua/convertor.py contents.xlsx  ./configs/client/ 1
python ./excel2Lua/convertor.py contents.xlsx  ./configs/server/ 0

rm -r ../../../Client/trunk/GameProject/Assets/Lua/Logic/Configs/*.lua

cp ./configs/client/*.lua ../../../Client/trunk/GameProject/Assets/Lua/Logic/Configs