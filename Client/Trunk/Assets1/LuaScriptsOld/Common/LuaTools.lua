LuaTools = {}

function LuaTools.TableDelete(array, value) 
	if array == nil then
		return
	end
	for i = #array, 1, -1 do
		if array[i] == value then
			table.remove(array,i)
			break
		end
	end
end 

function LuaTools.TableInsert(array, value) 
	if array == nil then
		return	
	end
	table.insert(array,value)
end 




