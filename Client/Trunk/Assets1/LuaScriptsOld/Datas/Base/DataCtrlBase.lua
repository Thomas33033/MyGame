--游戏逻辑处理模块基类
DataCtrlBase = class("DataCtrlBase")

function DataCtrlBase:ctor()
	self.ProtoMsgList = {}
end

function DataCtrlBase:Awake()

end

function DataCtrlBase:Update(dt) 

end

function DataCtrlBase:RegisterProtoMsg(msgName,msgCallback)
	NetworkManager.RegisterProtoMsg(msgName, msgCallback)
	table.insert(self.ProtoMsgList,msgName)
end

function DataCtrlBase:OnReconnect()

end

function DataCtrlBase:OnDestroy() 
  --this.sysTimer.OnDestory()
	for k,v in pairs(self.ProtoMsgList)  do
		NetworkManager.RemoveProtoMsg(k)
	end
end
