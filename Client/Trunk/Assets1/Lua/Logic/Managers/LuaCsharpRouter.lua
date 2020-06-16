--用于和C#中转;
LuaCsharpRouter = {};
local this = LuaCsharpRouter
--UI逻辑控制器
this.proxyList = {}

cjson = require("cjson")



GySystemInfo = Com.Cherray.GySystemInfo;
GyEncrypt = Com.Cherray.GyEncrypt;
Utils = Com.Cherray.GyUtility;
GyLayer = Com.Cherray.GyLayer;
ChatAnimator = Com.Cherray.ChatAnimator;
LuaUIManager = Com.Cherray.UIManager.Instance;
TimeManager = Com.Cherray.TimeManager.Instance;
NetManager =  Com.Cherray.NetManager.Instance;
EffectManager =  Com.Cherray.EffectManager.Instance;
BundleManager = Com.Cherray.BundleManager.Instance;
SDKManager = Com.Cherray.SDKManager.Instance;
NativeFuncManager = Com.Cherray.NativeFuncManager.Instance;
NotificationManager = Com.Cherray.NotificationManager.Instance;
LoadSceneManager = Com.Cherray.LoadSceneManager.Instance;
SoundManger = Com.Cherray.SoundManger.Instance
ESoundLayer = Com.Cherray.ESoundLayer
UnionPlatformManager = ByteDance.Union.UnionPlatformManager.Instance
RangersAppLogManager = ByteDance.AppLog.RangersAppLogManager
UIEventListener = UIEventListener;





--游戏初始化lua模块;
function LuaCsharpRouter.Startup()
	Network:RegisterPbs();--注册pb列表;
	LuaNetManager.RegisterListener()
end

--所有的准备工作完成;
function LuaCsharpRouter.OnAllFinished()
	if (not LuaUIManager:isWndOpened("GlobalWnd")) then
		LuaUIManager:OpenWndByLua("GlobalWnd");
	end
	UserStateController.Start();
	LuaUIManager:OpenWndByLua("LoginWnd");
	G_SysSettingProxy:LoadSetting()
end

function LuaCsharpRouter.BackToLoginUI()
	CloseAllUIExcept()
	UserStateController.Start();
	LuaUIManager:OpenWndByLua("LoginWnd");
end

--分发网络消息;
function LuaCsharpRouter.OnNetworkBrocast(messageID, messageSubID, message)
	Network:OnBrocast(messageID, messageSubID, message);
end


function LuaCsharpRouter.ChangeScene(sceneName, isShowSceneUI, callback)
	LuaUIManager:OpenWndByLua("ChangeSceneWnd");

	local notification = {};
 	notification.sceneName = sceneName;
 	notification.isShowSceneUI = isShowSceneUI;
 	notification.callback = callback;
 	GyNotify:SendNotification(CLIENT_NOTIFI_ID.CHANGE_SCENE, notification);
end

--C#传来的事件
function LuaCsharpRouter.OnFromCsharpNotification(strNotifyID, _arg1, _arg2, _arg3, _arg4, _arg5)
	local notifyID = CLIENT_NOTIFI_ID[strNotifyID];
	if notifyID then
		GyNotifyDispatch.OnFromCsharpNotification(notifyID, _arg1, _arg2, _arg3, _arg4, _arg5);
	else
		error("error strNotifyID:", strNotifyID)
	end
end

function LuaCsharpRouter.Collectgarbage()
	collectgarbage("collect");
end

