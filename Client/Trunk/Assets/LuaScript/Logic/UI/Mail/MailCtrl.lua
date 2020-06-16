G_MailCtrl = nil;
MailCtrl = class("MailCtrl", CtrlBase);


--初始化函数
function MailCtrl:OnInit()
	G_MailCtrl = self
	self.m_tempData = {}
	self.mailDataList = {}
	for i = 1,20,1 do
		LuaTools.TableInsert(self.mailDataList,{id=i, name="服务器"..i, bRead=false})
	end
end

--监听事件注册函数(来自网络层)
function MailCtrl:RegisterListener()
	--for example
	--self:RegisterNetWorkListener(PROTO_MSG.SC_TEMP, self.CS_Temp);
end

--每帧刷新
function MailCtrl:OnUpdate(dt)

end

-- 销毁清理函数
function MailCtrl:OnClear()
	self.m_tempData = nil
	G_MailCtrl = nil
end

function MailCtrl.OnCreate()
	return G_MailCtrl or MailCtrl.New()
end

