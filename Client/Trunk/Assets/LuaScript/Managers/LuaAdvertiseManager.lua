local cjson = require 'cjson'

LuaAdvertiseManager = {
    m_init = false
};

local this = LuaAdvertiseManager

function LuaAdvertiseManager:RegisterNotify()
    GyNotify:RegisterNotify("BD_OnRewardVideoCached", LuaAdvertiseManager.OnRewardVedioAdLoaded, this)
    GyNotify:RegisterNotify("BD_OnRewardVideoAdLoad", LuaAdvertiseManager.OnRewardVedioAdLoaded, this)
    GyNotify:RegisterNotify("BD_OnRewardVedioAdError", LuaAdvertiseManager.OnRewardVedioAdError, this)
    GyNotify:RegisterNotify("BD_OnRewardAdInteractionVerify", LuaAdvertiseManager.OnRewardVedioAdWatched, this)
end

function LuaAdvertiseManager.OnNotify(json)

    if not this.m_init then
        this.m_init = true
        this:RegisterNotify()
    end

    local result = cjson.decode(json)
    local event = result["event"]

    if GyNotify:HasNotify(event) then
        GyNotify:SendNotification(event, result)
    elseif string.find(event, "Error") then
        local message = result["message"] or ""
        local code = result["code"] or 0
        error(event.." [code]"..code.." [messge]"..message)
    else
        print(event)
    end
end

-- 激励视频广告
function LuaAdvertiseManager:ShowRewardVedioAd(rewardName, rewardAmount, extra)
    UnionPlatformManager:LoadRewardAd(cjson.encode({
		iosAdID = "940742784",
		androidAdID = "940729251",
		deepLink = true,
		imgWidth = 1080,
		imgHeight = 1920,
		rewardName = rewardName,
		rewardAmount = rewardAmount,
        userID = LuaPlayerDataManager.GetUUID(),
		extra = extra,
		orientation = 0,
	}))
end

function LuaAdvertiseManager:OnRewardVedioAdLoaded(result)
    UnionPlatformManager:ShowRewardAd()
end

-- 激励视频广告观看回调
function LuaAdvertiseManager:OnRewardVedioAdWatched(result)

    UnionPlatformManager:DisposeRewardAd()

    local verify = result["verify"]
    local name = result["name"]
    local amount = result["amount"]

    if verify == true then
        print("向服务器发送奖励")
    else
        print("没看完，不发奖励")
    end
end

-- 激励视频错误回调
function LuaAdvertiseManager:OnRewardVedioAdError(result)
    local message = result["message"]
    local code = result["code"]
    error("OnRewardVedioAdError [code]"..code.." [message]"..message)
end

-- Banner广告
function LuaAdvertiseManager:ShowBannerAd()
    UnionPlatformManager:LoadNativeBannerAd(cjson.encode({
        iosAdID = "940742784",
		androidAdID = "940729251",
		deepLink = true,
		imgWidth = 600,
        imgHeight = 257,
        adCount = 1,
    }))
end

-- 原生插屏广告
function LuaAdvertiseManager:ShowIntersititialAd()
    UnionPlatformManager:LoadNativeIntersititialAd(cjson.encode({
        iosAdID = "940742784",
		androidAdID = "940729251",
		deepLink = true,
		imgWidth = 600,
        imgHeight = 257,
        adCount = 1,
    }))
end

-- 全屏视频广告
function LuaAdvertiseManager:ShowFullScreenVideoAd()
    UnionPlatformManager:LoadFullScreenVideoAd(cjson.encode({
        iosAdID = "940742784",
		androidAdID = "940729251",
		deepLink = true,
		imgWidth = 1080,
        imgHeight = 1920,
		orientation = 0,
    }))
end