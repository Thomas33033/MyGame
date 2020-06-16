require "3rd/lua-protobuf/protoc"

Network = {
    pbManifestCache = {},
    messageMap = {},
    DEFAULT_MSG_NAME = ""
}

local this = Network;

function Network.GetProtoMsg(messageID)

    if Network.messageIdTable == nil then
        Network.messageIdTable = {}
        for k,v in pairs(PROTO_MSG) do
            Network.messageIdTable[v.messageID] = v;
        end
    end

    return Network.messageIdTable[messageID];
end

function Network.AddListener(proto, handler, _target)
    if proto == nil then
        return
    end

    local notifyID = proto.name;

    local currNotify = this.messageMap[notifyID];
    if (not currNotify) then
        currNotify = {};
        this.messageMap[notifyID] = currNotify;
    else
        for i,v in pairs(currNotify) do
            if (v.handler == handler) then return; end
        end
    end

    table.insert(currNotify, {handler = handler, target=_target});
end

function Network.RemoveListener(proto, handler)
    local notifyID = proto.name;

    local currNotify = this.messageMap[notifyID];
    if (not currNotify) then return; end

    for i,v in pairs(currNotify) do
        if (v.handler == handler) then
            table.remove(currNotify, i);
        end
    end
end

function Network.OnBrocast(messageID, message)

    local msgPack, err0 = pb.decode(PROTO_MSG.SC_TopLayer.name, message);
    if err0 then 
        print(err0, messageID, message); 
        return; 
    end

    local msgName = msgPack.MsgName;
    
    local notifyID = msgName;
    if this.messageMap[notifyID] == nil then
        warn("消息未注册:" .. notifyID)  
        return;
    end
    
    local reply, err = pb.decode(msgName, msgPack.Data);
    if err then 
        print(err, msgName); 
        return; 
    end

    local currNotify = this.messageMap[notifyID];
    if currNotify then
        for i,v in pairs(currNotify) do
            if msgName ~= "SC_Heartbeat" then
                print("收到消息:", msgName, reply);
            end
            if v.target then
                v.handler(v.target, reply);
            else
                v.handler(reply);
            end
           
        end
    end

    LuaNetManager:StopOffLineDelay();
    --luaEvent.Brocast(tostring(messageID), data);
end

function Network:OnBrocastByLua(notifyID, reply)
    local currNotify = this.messageMap[notifyID];
    if currNotify then
        for i,v in pairs(currNotify) do
            v.handler(reply);
        end
    end
end

function Network:RegisterPbs()

    -- init pb option begin
    -- reference https://github.com/starwing/lua-protobuf
    pb.option("enum_as_value")
    -- init pb option end

    for i,v in ipairs(PbManifest) do
        if not Network:checkCacheHave(v) then
            pb.load(Utils.LuaReadFile("Pbs/"..v))
        end
    end

end

function Network:checkCacheHave(fileName)
    if not this.pbManifestCache then
        return false;
    else
        for i,v in ipairs(this.pbManifestCache) do
            if (v == fileName) then
                return true;
            end
        end
    end

    return false;
end

function Network:Send(proto, msg)
    if (proto ~= nil) then
        if proto.name ~= "CS_Heartbeat" then
            print("发送消息:", proto.name, msg);
        end
    else
        return;
    end

    local realData = {}
    realData.MsgName = proto.name;
    realData.Data = pb.encode(proto.name, msg);
    local realMsg = pb.encode(PROTO_MSG.CS_TopLayer.name, realData);
    NetManager:SocketSendByLua(proto.messageID, realMsg);
end