

--打印字符串--
require("Logic/debugPrint")


--连接两个数组
function ConcatTableArray(...)
	local args = {...}
	local result = {}
	for i=1, #args do
		local oneTable = args[i]
		if oneTable then
			for j=1, #oneTable do
				table.insert(result, oneTable[j])
			end
		end
  	end
  	return result;
end

function newUnpack(t,i)
	i=i or 1;
	if (i <= table.maxn(t))
	then
		return t[i],newUnpack(t, i+1);
	end
end

function IsNil(obj)
	if (type(obj) == "userdata") then
		return obj:Equals(nil);
	else
		if (not obj) then
			return true;
		end
	end

	return false;
end

function SetActive(object, isActive)
	if IsNil(object) then
		return;
	else
		if object.gameObject.activeSelf ~= isActive then
			object.gameObject:SetActive(isActive);
		end
	end
end

function ClearTableGameObjects(tbl)
	for i = #tbl, 1, -1 do
		GameObject.DestroyImmediate(tbl[i]);
		table.remove(tbl, i);
	end
	tbl = {};
end

function ClearChild(rootT)
	if IsNil(rootT) then
		return;
	end
	local childCount = rootT.childCount;
	for i = childCount, 1, -1 do
		local curPoint = rootT:GetChild(i - 1);
		if not IsNil(curPoint) then
			GameObject.Destroy(curPoint.gameObject);
		end
	end
end

function StringSplit(src, delimiter)
	if (src == nil or delimiter == nil)
	then
		return nil;
	end
	
	local result = {};
	
	for match in (src..delimiter):gmatch("(.-)"..delimiter) do
		table.insert(result, match);
	end
	
	return result;
end

function NumToNormal(num)
	local norNum = num;
	if (num >= 100000000) then
		norNum = (math.floor(num / 10000000) / 10).."亿";
	-- elseif (num >= 100000) then
	-- 	norNum = (math.floor(num / 1000) / 10).."万";
	end

	return norNum;
end
----------------------------------------------------------------------------------------------------
--创建一个子组件
function CreateOneItem(item, parentT)
	local itemObj = GameObject.Instantiate(item);
	local t = itemObj.transform;
	t:SetParent(parentT);
	t.localPosition = Vector3.zero;
	t.localScale = Vector3.one;

	return itemObj;
end


----------------------------------------------------------------------------------------------------

function ReadOnly(t)
	local proxy = {}
	local mt = {       -- create metatable;
		__index = t,
		__newindex = function (t,k,v)
			error("attempt to update a read-only table")
		end
	}
	setmetatable(proxy, mt)
	return proxy
end

function OpenShareImgMenu(imgPath)
	ShareSDKManager.Instance:OpenShareImgMenu(imgPath,System.Action_object(
		function(obj)
			GyNotify:SendNotification(CLIENT_NOTIFI_ID.SHARE_SUCCESS, nil);
	end));
end

function OpenShareMenu()
	RequestShareShortUrl(GetShareUrl(), function(shorUrl, result)
		-- body
		local text = "来吧，让我们互相伤害吧，这个夏天，一起怦怦";
		local imageUrl = "http://download.bh.ztgame.com.cn/bigboom/bigboom.png";
		local title = "怦怦英雄 来吧互相伤害吧";
		local titleUrl = shorUrl
		local site = "怦怦英雄";
		local siteUrl = shorUrl
		local url = shorUrl
		local comment = "横版投掷类游戏，你可以用炮弹伤害、击飞对手，还能挖坑让对手出不来，你会巧妙的把对手推下海么？这个夏天，一起怦怦！";
		local musicUrl = "";
		local sinaText = "怦怦英雄 来吧互相伤害吧";
		local sinaImageUrl = "http://download.bh.ztgame.com.cn/bigboom/bigboom.png";
		ShareSDKManager.Instance:OpenShareMenu(text, imageUrl, title, titleUrl, site, siteUrl, url, comment, musicUrl, sinaText, sinaImageUrl, System.Action_object(
			function(obj)
				GyNotify:SendNotification(CLIENT_NOTIFI_ID.SHARE_SUCCESS, nil);
		end));
	end)
end

function GetShareUrl()
	--"http://ppyx.ztgame.com/";
	--118.194.49.5
	if GMToolsProxy.changeShareUrl then
		return string.format("http://118.194.49.5/ShareUrl?UID=%s&t=%s", Player.user_id or 0, os.time()) 
	end
    return string.format("http://shareppyx.playnb.net/ShareUrl?UID=%s&t=%s", Player.user_id or 0, os.time())
end

--上新浪平台请求短链接  callback(shorUrl, result)
function RequestShareShortUrl(longUrl, callback)
  	StartCoroutine(
	  	function()
	        local www = UnityEngine.WWW("http://api.t.sina.com.cn/short_url/shorten.json?source=1896739018&url_long="..longUrl)
	        Yield(www)
	        if string.len(www.text) > 0 then
	            local shortUrl = Utils.ParseShortUrlFromJson(www.text)
	            if shortUrl ~= nil and shortUrl ~= "" then
	                callback(shortUrl, true)
	            else
	                callback(longUrl, false)
	            end
	        else
	            callback(longUrl, false)
	        end
	    end
    )
end


function trim(str)
	return (string.gsub(str,"^%s*(.-)%s*$","%1"))
end

