local JSON = require "3rd/json"

function ToJsonString(v)
	return JSON:encode(v)
end

function ToJsonTable(v)
	return JSON:decode(v)
end

--输出错误日志--
function error(...)
	local str = "";
	local data = {...}
	local num = #data;
	for i=1,num do
		str = str .. data[i] .. " ";
	end

	str = str.."\n" .. debug.traceback();
	LuaInterface.Debugger.LogError(str);
end


--连接两个数组
function ConcatTableArray(...)
	local args = {...}
	local result = {}
	for i=1, #args do
		local oneTable = args[i]
		if oneTable then
			for j=1, #oneTable do
				table.insert(result, oneTable[j])
			end
		end
  	end
  	return result;
end

function newUnpack(t,i)
	i=i or 1;
	if (i <= table.maxn(t))
	then
		return t[i],newUnpack(t, i+1);
	end
end

function IsNil(obj)
	if (type(obj) == "userdata") then
		return obj:Equals(nil);
	else
		if (not obj) then
			return true;
		end
	end

	return false;
end

function SetActive(object, isActive)
	if IsNil(object) then
		return;
	else
		if object.gameObject.activeSelf ~= isActive then
			object.gameObject:SetActive(isActive);
		end
	end
end

function ClearTableGameObjects(tbl)
	for i = #tbl, 1, -1 do
		GameObject.DestroyImmediate(tbl[i]);
		table.remove(tbl, i);
	end
	tbl = {};
end

function ClearChild(rootT)
	if IsNil(rootT) then
		return;
	end
	local childCount = rootT.childCount;
	for i = childCount, 1, -1 do
		local curPoint = rootT:GetChild(i - 1);
		if not IsNil(curPoint) then
			GameObject.Destroy(curPoint.gameObject);
		end
	end
end

function StringSplit(src, delimiter)
	if (src == nil or delimiter == nil)
	then
		return nil;
	end
	
	local result = {};
	
	for match in (src..delimiter):gmatch("(.-)"..delimiter) do
		table.insert(result, match);
	end
	
	return result;
end

function NumToNormal(num)
	local norNum = num;
	if (num >= 100000000) then
		norNum = (math.floor(num / 10000000) / 10).."亿";
	-- elseif (num >= 100000) then
	-- 	norNum = (math.floor(num / 1000) / 10).."万";
	end

	return norNum;
end
----------------------------------------------------------------------------------------------------
--创建一个子组件
function CreateOneItem(item, parentT)
	local itemObj = GameObject.Instantiate(item);
	local t = itemObj.transform;
	t:SetParent(parentT);
	t.localPosition = Vector3.zero;
	t.localScale = Vector3.one;

	return itemObj;
end


----------------------------------------------------------------------------------------------------

function ReadOnly(t)
	local proxy = {}
	local mt = {       -- create metatable;
		__index = t,
		__newindex = function (t,k,v)
			error("attempt to update a read-only table")
		end
	}
	setmetatable(proxy, mt)
	return proxy
end

function OpenShareImgMenu(imgPath)
	ShareSDKManager.Instance:OpenShareImgMenu(imgPath,System.Action_object(
		function(obj)
			GyNotify:SendNotification(CLIENT_NOTIFI_ID.SHARE_SUCCESS, nil);
	end));
end


function trim(str)
	return (string.gsub(str,"^%s*(.-)%s*$","%1"))
end

function GetOrAddComponent(obj,comp)


	uiTable.luaUI = obj:AddComponent(comp)

end
