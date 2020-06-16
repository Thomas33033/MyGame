

PRINT_SOURCE = true
CWD = "" -- you can set a global value CWD to current working directory and the log will include it
PRINT_STACK = true
PRINT_MM_TIME = false

local oldprint = print



local function packArg(...)
  local str = ""
  local args = {...}
  local n = #args;
  for i=1, n do
    str = str..table2string(args[i], i==1)
  end
  return str
end

print = function(...)
  local str = ""
  if PRINT_SOURCE then
    local info = debug.getinfo(2, "Sl") -- print function is call stack 1, and the caller is 2
    local source = info and info.source
    if source then
      str = string.format("%s(%d,1) %s", source, info.currentline, packArg(...))
    else
      str = packArg(...)
    end
  else
    str = packArg(...)
  end

  if PRINT_STACK then
    str = str.."\n" .. debug.traceback();
  end

  -- oldprint(str)
  log(str)
end


--输出日志--
function log(str)
    LuaInterface.Debugger.Log(str);
end


--输出错误日志--
function error(...)
  local str = "";
  local data = {...}
  local num = #data;
  for i=1,num do
    str = str .. table2string(data[i]) .. " ";
  end

  str = str.."\n" .. debug.traceback();
  LuaInterface.Debugger.LogError(str);
end


--警告日志--
function warn(...) 
  print("<color=#CC6600>" .. packArg(...) .. '</color>')  
end

function table2string ( t )  
    local showTableId = false;
    local print_r_cache={}
    local str = ""
    local showEnter = (type(t)=="table");
    local function sub_print_r(t, indent)
        if (type(t)=="table") then
            for pos,val in pairs(t) do
                if (type(val)=="table") then
                    str = str .. (indent.."\"<color=#00CC66>"..pos.."</color>\" = ".. (showTableId and tostring(t) or "").." {") .. (showEnter and "\n" or "");
                    sub_print_r(val, indent..string.rep(" ",string.len(pos)+4))
                    str = str .. (indent..string.rep(" ",string.len(pos)+2).."}" .. ",") .. "\n"
                elseif (type(val)=="string") then
                    str = str .. (indent.."\"<color=#00CC66>"..pos..'</color>\" = "'..val..'"' .. ",") .. (showEnter and "\n" or "");
                else
                    str = str .. (indent.."\"<color=#00CC66>"..pos.."</color>\" = "..tostring(val) .. ",") .. (showEnter and "\n" or "");
                end
            end
        else
            str = str .. (indent..tostring(t)).. (showEnter and "\n" or " ");
        end
    end

    if (type(t)=="table") then
        str = str .. (tostring(t).." {") .. "\n"
        sub_print_r(t, "  ")
        str = str .. ("}") .. "\n"
    else
        sub_print_r(t, "")
    end
    
    return str
end

function ShowTB(p_table)
  PrintLongString(table2string(p_table));
end

--打印一个长字符串
function PrintLongString(p_strLog)
  local singleLogLen = 15700;
  local count = math.ceil(string.len(p_strLog) / singleLogLen);
  for i=1,count do
  local temp = string.sub(p_strLog, (i-1) * singleLogLen + 1, i*singleLogLen);
      print(temp);
  end
end




--测试用例
-- local studentNum = {1, {6, 7, {"hello", "world"}}, 5}
-- studentNum["xxx"] = {1,2,3}
-- print(studentNum)