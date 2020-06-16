
GyNotifyDispatch = {}

function GyNotifyDispatch.Initialize()
	
end

function GyNotifyDispatch.Dispose()
	
end

function GyNotifyDispatch.OnFromCsharpNotification(notifyID, _arg1, _arg2, _arg3, _arg4, _arg5)
	if (notifyID == CLIENT_NOTIFI_ID.C_SOCKET_EXCEPTION) then
		LuaNetManager:CsharpSocketException();
	elseif (notifyID == CLIENT_NOTIFI_ID.C_FORNETWORK) then
		LuaNetManager:AllDelayHandle(_arg1);
	elseif (notifyID == CLIENT_NOTIFI_ID.LAST_SYNC_DIFF_SECOND) then
		LuaNetManager:ReqServerTime();
		if (tonumber(_arg1) > 120) then
			LuaNetManager:CsharpSocketException();
		end
		GyNotify:SendNotification(notifyID, {arg1 = _arg1, arg2 = _arg2, arg3 = _arg3, arg4 = _arg4, arg5 = _arg5});
	elseif (notifyID == CLIENT_NOTIFI_ID.KEYCODE_ESCAPE) then
		local func = function() UnityEngine.Application.Quit(); end
		LuaCsharpRouter.ShowBox("确定退出？", func, nil, nil, nil, false);
	else
		GyNotify:SendNotification(notifyID, {arg1 = _arg1, arg2 = _arg2, arg3 = _arg3, arg4 = _arg4, arg5 = _arg5});
	end
end


