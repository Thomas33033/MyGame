Event = {}

local dic = {}

function Event.Add(type, func)
    if dic[type] == nil then
        dic[type] = {}
    end
    table.insert(dic[type],func)
end

function Event.Clear(type)
    dic[type] = nil
end

function Event.Remove(type,func)
    if dic[type] == nil then
        return
    end
    for i,v in ipairs(dic[type]) do
        if v == func then
            table.remove(dic[type],i)
            break
        end
    end
end


function Event.Call(type,... )
    if dic[type] == nil then
        return
    end
    for i=#dic[type],1,-1 do
        if dic[type] == nil then
            break
        end
        if dic[type][i] ~= nil then
            dic[type][i](type,...)
        end
    end
end



EventType = {
    LoadUI = "LoadUI",
    TimeCountOver = "TimeCountOver",
}