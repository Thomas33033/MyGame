--本地数据存储Key,全局唯一不能重复

LocalDataKey = {
    EAccount = "Account",
    EAccountType = "AccountType",
	EServerId = "ServerId",
	EOpenSound = "OpenSound",
	EOpenMusic = "OpenMusic",
	EOpenHighDPI = "OpenHighDPI",
	EOpenHighModel = "OpenHightModel",
	EOpenHighFrame = "OpenHighFrame",
	EOpenFightEffect = "OpenHighEffect"
}

LocalStorage = {}
--获取本地账号
function LocalStorage.GetAccount()
	Player.account = PlayerPrefs.GetString(LocalDataKey.EAccount, "")
	return Player.account
end

function LocalStorage.SetAccount(account)
	PlayerPrefs.SetString(LocalDataKey.EAccount, account)
end

--获取本地账号类型
function LocalStorage.GetAccountType()
	Player.accountType = tonumber(PlayerPrefs.GetString(LocalDataKey.EAccountType, tostring(ELoginAccountType.None)))
	return Player.accountType
end

function LocalStorage.SetAccountType(accountType)
    Player.accountType = accountType
	PlayerPrefs.SetString(LocalDataKey.EAccountType, accountType)
end

--获取本地服务器
function LocalStorage.GetServerId()
	Player.SERVER_ID = PlayerPrefs.GetString(LocalDataKey.EServerId, "")
	return  Player.SERVER_ID
end

--设置服务器ID
function LocalStorage.SetServerId(serverId)
	Player.SERVER_ID = serverId
	PlayerPrefs.SetString(LocalDataKey.EServerId, serverId)
end

--获取音效开关
function LocalStorage.GetOpenSoundState()
	local isOpen = PlayerPrefs.GetString(LocalDataKey.EOpenSound, "")
	if LuaTools.IsNullOrEmpty(str) then
		return "1"
	end
	return isOpen
end

--设置音效开关
function LocalStorage.SetOpenSoundState(isOpen)
	PlayerPrefs.SetString(LocalDataKey.EOpenSound, isOpen.."")
end


--获取音乐开关
function LocalStorage.GetOpenMusicState()
	local isOpen = PlayerPrefs.GetString(LocalDataKey.OpenMusic, "")
	if LuaTools.IsNullOrEmpty(str) then
		return "1"
	end
	return isOpen
end

--设置音乐开关
function LocalStorage.SetOpenMusicState(isOpen)
	PlayerPrefs.SetString(LocalDataKey.OpenMusic, isOpen.."")
end

--获取开关设置
function LocalStorage.GetBooleanSetting(strkey, defaultValue)
	if defaultValue == true then
		return PlayerPrefs.GetInt(strkey, 1) == 1
	else
		return PlayerPrefs.GetInt(strkey, 0) == 1
	end
end

--设置开关
function LocalStorage.SetBooleanSetting(strkey, value)
	if value == true then
		PlayerPrefs.SetInt(strkey, 1)
	else
		PlayerPrefs.SetInt(strkey, 0)
	end
end
