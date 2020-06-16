Util = {}

function Util.TableDelete(array, value) 
	for i = #array, 1, -1 do
		if array[i] == value then
			table.remove(array,i)
			break
		end
	end
end 

function Util.TableAdd(array, value) 
	table.insert(array,value)
end 




