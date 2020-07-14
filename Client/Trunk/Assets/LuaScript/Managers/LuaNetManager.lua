
require "3rd/Network/Network"

LuaNetManager = {};
local this = LuaNetManager

this.m_serverTime = 0; --服务器时间
this.m_receiveMsgTime = 0; --接收服务器心跳包时间
this.m_receiveMsgTimeOutTime = 0; --接收服务器心跳包的超时时间
this.TimeOutTime = 6 --心跳包超时时间秒
this.KeepAliveInterval = 3 --每隔30秒向服务器请求一次
this.m_pingTimes = 0;  

local function ConnetServerSuccessCallBack()

	LuaUIManager:CloseWndByLua("WaitingWnd");

	NetManager.isConnectServerSuccess = true;

    GyNotify:SendNotification(CLIENT_NOTIFI_ID.LOGIN_REQ, nil);
	LuaNetManager:ReqServerTime();

end

local function ReconnetServerfailCallBack()
	LuaCsharpRouter.ShowBox("重新连接服务器失败，请检查网络连接。", function() LuaNetManager:ReConnectLobbyServer(); end, nil, nil, nil, true);
	NetManager.isConnectServerSuccess = false;
end

local function ConnetServerfailCallBack()
	LuaCsharpRouter.ShowBox("与服务器连接丢失,将尝试重新连接。", function() LuaNetManager:OffLineHandle(); end, nil, nil, nil, false);
	NetManager.isConnectServerSuccess = false;
end

local function ConnectLobbyServerWithFlag(p_isReconnect)

	local successCallBack = System.Action(ConnetServerSuccessCallBack);
	local failCallBack = nil;
	if p_isReconnect then
	 	failCallBack = System.Action(ReconnetServerfailCallBack);
	else
		failCallBack = System.Action(ConnetServerfailCallBack);
	end
	if (Player.DOMAIN_FLAG == 1) then
    	NetManager:SocketConnect(Player.IP, Player.PORT, successCallBack, failCallBack);
	elseif (Player.DOMAIN_FLAG == 2) then
		NetManager:SocketConnectByDomain(Player.IP, Player.PORT, successCallBack, failCallBack);
	end
end

-------------------------服务器心跳时间管理 结束-----------------------------------------
function LuaNetManager.RegisterListener()
	--注册帧监听
	Network.AddListener(PROTO_MSG.SC_Heartbeat, LuaNetManager.SC_Heartbeat);
end

function LuaNetManager.RemoveListener()
	Network.RemoveListener(PROTO_MSG.SC_Heartbeat, LuaNetManager.SC_Heartbeat);
end

function LuaNetManager.SC_Heartbeat(msgData)
	this.m_serverTime = math.floor(msgData.ServerTimeMs / 1000) 
	this.m_receiveMsgTime = Utils.GetNowSecond()
	this.m_receiveMsgTimeOutTime = 0
	if EHeartbeatTypes.HEARTBEAT_PING == msgData.Type then
		this.CS_Heartbeat(EHeartbeatTypes.HEARTBEAT_PONG)
	end
end

--获取服务器时间
function LuaNetManager.GetServerTime()
	 local nowSeconds = Utils.GetNowSecond()
	 return nowSeconds - this.m_receiveMsgTime + this.m_serverTime
end

--EHeartbeatTypes.HEARTBEAT_PING
--客户请求服务器心跳
function LuaNetManager.CS_Heartbeat(type)
	local msgData = { }
	msgData.Type = type
    Network:Send(PROTO_MSG.CS_Heartbeat, msgData)
end

function LuaNetManager:Update()
	--玩家已经登录
	if LuaPlayerDataManager.LoginSuccess ~= true then
		return;
	end
	local dt = Time.deltaTime
	this.m_pingTimes = this.m_pingTimes + dt
	this.m_receiveMsgTimeOutTime = this.m_receiveMsgTimeOutTime + dt

	if this.m_pingTimes > this.KeepAliveInterval then
		this.m_pingTimes = 0
		this.CS_Heartbeat(EHeartbeatTypes.HEARTBEAT_PING)
	end

	if this.m_receiveMsgTimeOutTime > this.TimeOutTime then
		warn("服务器超时")
		if (not LuaUIManager:isWndOpened("WaitingWnd")) then
			if NetManager.isConnectServerSuccess == true then
				LuaUIManager:OpenWndShowFrontByLua("WaitingWnd");
			end
		end


		LuaNetManager:OffLineHandle();	--切到选人界面启动重连p

		this.m_receiveMsgTimeOutTime = 0
	end
end

-----------------------------服务器心跳时间管理 结束---------------------------------------------


--请求连接登录服
function LuaNetManager:ConnectLobbyServer()
	ConnectLobbyServerWithFlag(false);
end

--请求重新连接登录服
function LuaNetManager.ReConnectLobbyServer()
	ConnectLobbyServerWithFlag(true);
end


--请求服务器时间;
function LuaNetManager:ReqServerTime()
	if (NetManager.isConnectServerSuccess) then
		LuaNetManager:StopOffLineDelay();
		NetManager:StartShowWaitingDelay(3); --请求时间3s后打开转圈，如果已经收到服务器返回时间则会stop掉这个事件
	end
end

--服务器返回时间;
function LuaNetManager:RetServerTime()
	LuaNetManager:StopOffLineDelay();
	NetManager:StartReqHeartDelay(6);   --收到服务器时间后开启6s倒计时，6s以后再次请求服务器时间(除非socket已经断开或者已经esc退出会stop掉这个delay)
end

function LuaNetManager:StopOffLineDelay()
	NetManager:StopOffLineDelay();
	NetManager:StopShowWaitingDelay();

	GyNotify:SendNotification(CLIENT_NOTIFI_ID.CLOSE_WAITING, nil);
end

function LuaNetManager:AllDelayHandle(tag)
	if (tag == NET_TAG.REQ_HEART) then
		LuaNetManager:ReqServerTime();
	elseif (tag == NET_TAG.OFF_LINE) then
		LuaNetManager:CsharpSocketException();
	elseif (tag == NET_TAG.SHOW_WAITING) then
		if (not LuaUIManager:isWndOpened("WaitingWnd")) then
			if (NetManager.isConnectServerSuccess) then
				LuaUIManager:OpenWndShowFrontByLua("WaitingWnd");
			end
		end
		
		NetManager:StartOffLineDelay(10);  --打开转圈以后开启10s倒计时，如果有收到服务器时间或者请求过服务器时间则StopOffLineDelay，同时关掉转圈，超时未收到则认为CsharpSocketException
	end
end

--------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------

function LuaNetManager:Quit()

	--退出后最近登陆过的账号改为游客账号  只有编辑器生效
	if (Utils.isUnityEditor() or Utils.IsUnityWindowsPlatform()) then
		local playerPrefsKey = PlayerPrefsStrings.LAST_ACCOUNT_ID .. UnityEngine.Application.dataPath;
		PlayerPrefs.SetString(playerPrefsKey, Utils.deviceUniqueIdentifier);
	end
	
	LuaNetManager:StopOffLineDelay();

	GyNotify:SendNotification(CLIENT_NOTIFI_ID.OFF_LINE_HANDLE, nil);

	NetManager:StopReqHeartDelay();

	NetManager:SocketDispose();
	NetManager.isConnectServerSuccess = false;

	CloseAllUIExcept(nil);

	LuaUIManager:OpenWndByLua("ServerWnd");
end
--------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------

function LuaNetManager:CsharpSocketException()
	-- if (NetManager.isConnectServerSuccess) then
	-- 	NetManager:StopReqHeartDelay();
	-- 	LuaNetManager:StopOffLineDelay();
	-- 	LuaNetManager:OffLineHandle();
	-- end
end

function LuaNetManager:OffLineHandle()
	LuaPlayerDataManager.LoginSuccess = false;
	NetManager.isConnectServerSuccess = false;
	GyNotify:SendNotification(CLIENT_NOTIFI_ID.OFF_LINE_HANDLE, nil);
	LuaBattleController:Clear();
	UserStateController.ReStart();
	-- LuaNetManager:ReConnectLobbyServer();
	LuaCsharpRouter.ChangeScene("entry", false, function() 
		NetManager:SocketDispose();
		NetManager.isConnectServerSuccess = false;
		CloseAllUIExcept(nil);
		LuaUIManager:OpenWndByLua("LoginWnd");
		LuaUIManager:SetActiveStartupCanvas(true);
	end);
end


