require "Common/MsgDefine"

local pb = require "pb"
local protoc = require "ThirdParty/lua-protobuf/protoc"
serpent = require "ThirdParty/lua-protobuf/serpent"

NetworkManager = {}
local this = NetworkManager
this.LuaNetWorkManager = LuaNetWorkManager.Instance;
this.MsgCallbackSet = {}


this.str_ip = "192.168.116.246";
this.i_port = 11002;

function NetworkManager:LoadProtoPB()
	for k,v in ipairs(pbFileSets) do
		assert(pb.loadfile(AppConst.dataPath..v))
	end
end

function NetworkManager.RemoveProtoMsg(msgName)
    if (this.MsgCallbackSet[msgName] ~= nil) then
		this.MsgCallbackSet[msgName] = nil
	end
end

function NetworkManager.RegisterProtoMsg(msgName,msgCallback)
	if (this.MsgCallbackSet[msgName] == nil) then
		this.MsgCallbackSet[msgName] = msgCallback;
	end
end

function NetworkManager.ReceiveProtoMsg(bytedata)
	local topLayerMsgProto = pb.decode("SC_TopLayer", bytedata)
	local msgName = topLayerMsgProto.MsgName
	local msgData = topLayerMsgProto.Data
	local msgCallback = this.MsgCallbackSet[msgName]
	if msgCallback ~= nil then
		local msgProto = assert(pb.decode(msgName, msgData));
		if msgProto == nil then
			error('failed to decode msg:', msgName)
		else
			print("接收消息:".. msgName.."\n"..serpent.block(msgProto))
			msgCallback(msgProto)
		end
	else
		error('receive unregisger message:', msgName);
	end
end

-- 向服务器发送消息
function NetworkManager.SendProtoMsg(msgName, msgProto)
	local topLayerMsgProto = {
		MsgName = msgName,
		Data = pb.encode(msgName, msgProto or {}),
	}
	print("发送消息:".. msgName.."\n"..serpent.block(msgProto))
	local bytedata = pb.encode("CS_TopLayer", topLayerMsgProto)
	this.LuaNetWorkManager:SendMsg(0, bytedata);
end

function NetworkManager.OnReconnection()
	 this.LuaNetWorkManager:OnReconnection(this.str_ip,this.i_port)
end

function NetworkManager.OnConnectionOk()
	G_GameDataCtrl:OnConnectionOk()
end


this.callback = NetworkManager.ReceiveProtoMsg;

return this