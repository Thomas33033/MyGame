LuaTools = {}
LuaTools.COLOR_RED = "#ff0000"
LuaTools.COLOR_GREEN = "#31E243"
LuaTools.COLOR_WHITE = "#ffffff"

ITEM_NAME_COLOR = {}
ITEM_NAME_COLOR[1] = "#C8C8C8"
ITEM_NAME_COLOR[2] = "#11FF00"
ITEM_NAME_COLOR[3] = "#00EDFF"
ITEM_NAME_COLOR[4] = "#FF50F3"
ITEM_NAME_COLOR[5] = "#FFB648"
ITEM_NAME_COLOR[6] = "#FF0000"

ITEM_NAME_TEXT = {}
ITEM_NAME_TEXT[1] = "普通"
ITEM_NAME_TEXT[2] = "优秀"
ITEM_NAME_TEXT[3] = "稀有"
ITEM_NAME_TEXT[4] = "史诗"
ITEM_NAME_TEXT[5] = "传说"
ITEM_NAME_TEXT[6] = "神话"

ESOUND_NAME= {}
ESOUND_NAME.BG = 1
ESOUND_NAME.BOSS_COME = 2
ESOUND_NAME.BOSS_FIGHT = 3
ESOUND_NAME.BATTLE_VICTORY = 4
ESOUND_NAME.CLICK_BAG = 5
ESOUND_NAME.CLICK_INVALID = 6
ESOUND_NAME.CHANGE_EQUIP = 7
ESOUND_NAME.FORGE_EQUIP = 8
ESOUND_NAME.CLICK_SMALL_MAP = 9
ESOUND_NAME.SKILL_LEVLE_UP = 10
ESOUND_NAME.CLICK_COMMON = 11
ESOUND_NAME.GET_AWARD = 42
ESOUND_NAME.DIALOG_BOX = 43



Vector2.right = Vector2.New(1, 0)
Vector2.left = Vector2.New(-1, 0)
Vector2.up = Vector2.New(0, 1)
Vector2.down = Vector2.New(0, -1)
Vector2.zero = Vector2.New(0, 0)


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


function LuaTools.IsNullOrEmpty(str)
	if str == nil or str == "" then
		return true;
	else
		return false
	end
end

function LuaTools.GetMapLength(map)
	local count = 0
	for v,k in pairs(map)  do
		count = count + 1
	end
	return count
end

--秒转化为时间描述 年、月、日、时、分
function LuaTools.SecondTimeToSingleTimeStr(seconds)
	local str = ""
	if seconds < 60 then
		str = seconds .."秒"
	elseif seconds > 60 and seconds < 3600 then
		str = math.floor(seconds/60).."分钟"
	elseif seconds > 3600 then
		str = math.floor(seconds/3600).."小时"
	end
	return str
end

--格式:2019.12.19
function LuaTools.TimeToEnString(milliseconds)
	return os.date("%Y.%m.%d",milliseconds)
end


--基于锚点对齐UI
function LuaTools.AnchorTo(targetRectTrans, targetAnchor,sourceRectTrans, sourceAnchor,offset)
	local target = targetRectTrans
	local source = sourceRectTrans
	if target == nil or source == nil then
		return
	end
	error(targetAnchor)
	Utils.AnchorTo(targetRectTrans, targetAnchor,sourceRectTrans, sourceAnchor,offset)
end

function LuaTools.SetGray(imageObj,showGray)
	if grayMaterial==nil then
		grayMaterial = LuaUIManager:LoadMaterial("Shaders/GrayMaterial")
	end
	if showGray == true then
		imageObj.material = grayMaterial;
	else
		imageObj.material = nil;
	end
end

--字符颜色处理
function LuaTools.ChangeStringColor(str,color)
	local content = ""
	if str ~=nil and color ~=nil then
	content =  "<color="..color..">"..str.."</color>"
	end
	return content
end


function LuaTools.GetJobId(job)
	return "10"..job.."01"
end



