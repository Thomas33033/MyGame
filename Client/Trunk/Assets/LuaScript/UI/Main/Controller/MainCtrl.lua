MainCtrl = {};
CtrlTable = class("MainCtrl", CtrlBase);

function MailCtrl.OnCreate()
	local ctrl = CtrlTable.New()
	ctrl: OnInit()
	return ctrl
end

function CtrlTable:OnInit()
end

function CtrlTable:RegisterListener()
	--for example
	--self:RegisterNetWorkListener(PROTO_MSG.SC_TEMP, self.CS_Temp);
end

function CtrlTable:OnUpdate(dt)

end

function CtrlTable:OnClear()
end

return MainCtrl
