
UIHelper = {}
local this = UIHelper


--获取图片组件
function UIHelper.SetImageAlpha(p_image, p_alpha)
	if not IsNil(p_image) then
		local oldColor = p_image.color;
		p_image.color = Color.New(oldColor.r, oldColor.b, oldColor.g, p_alpha);
	end
end

--获取图片组件
function UIHelper.GetImage(obj,path)
	local obj = this.GetObject(obj,path)
	if not obj then
		return
		end
	return obj:GetComponent("Image");
end


--获取文本组件InputField
function UIHelper.GetInputField(obj,path)
	local obj = this.GetObject(obj,path)
	if not obj then
		return
	end
	return obj:GetComponent("InputField");
end

--获取文本组件Text
function UIHelper.GetText(obj,path)
	local obj = this.GetObject(obj,path)
	if not obj then
		error("不能存在路径:"..path)
		return
	end
	return obj:GetComponent("Text");
end

--获取文本组件RectTransform
function UIHelper.GetRectTransform(obj,path)
	local obj = this.GetObject(obj,path)
	if not obj then
		error("不能存在路径:"..path)
		return
	end
	return obj:GetComponent("RectTransform");
end


--获取列表组件
function UIHelper.GetList(obj,path)
	local obj = this.GetObject(obj,path)
	if not obj then
		return
	end
	return obj:GetComponent("List");
end

--获取按钮组件
function UIHelper.GetButton(obj,path)
	local obj = this.GetObject(obj,path)
	if not obj then
		return
	end
	return obj:GetComponent("Button");
end

-- 检查界面是否存在
function UIHelper.GetObject(obj,path)

	if type(path) ~= 'string' then
		error("警告： objName:" .. path .. " is not string。")
		return nil
	end

	if not obj  or  not obj.transform  then
		error("警告： 父节点为空:")
		return nil
	end
	local child =  obj.transform:Find(path)
	if not child then
		error(path .. "不存在请检查，界面名称:")
	end
	return child
end

--默认播放音效
function UIHelper.AddButtonClick(obj,func,target)
	UITools.AddClickEvent(obj.gameObject, function(obj)
		--LogicTools.PlayAudio(ESOUND_NAME.CLICK_COMMON)
		error("==1==")
		if func then
			func(target,obj)
		end
	end);
end

--没有点击音效
function UIHelper.AddButtonClickOnSound(obj,func)
	UITools.AddClickEvent(obj.gameObject, function(obj)
		if func then
			func(obj)
		end
	end);
end


function UIHelper.AddButtonDown(obj,func)
	UIEventListener.Get(obj.gameObject).onDown =  function(obj)
		if func then
			func(obj)
		end
	end;
end

function UIHelper.AddButtonUp(obj,func)
	UIEventListener.Get(obj.gameObject).onUp =  function(obj)
		if func then
			func(obj)
		end
	end;
end

function UIHelper.AddToggleClick(obj,func)
	LuaUIManager:AddListener(obj.gameObject,"Toggle", func);
end

function UIHelper.SetActive(obj,bActive)
	if IsNil(obj) or IsNil(obj.gameObject) then
		error("对象为空")
		return
	end
	obj.gameObject:SetActive(bActive)
end

-- 设置单张大图
-- @example UIHelper.SetAssetBundleSpriteSingle(m_imageIcon, LuaBundlePathString.SingleImage_HeadIcon_1001)
function  UIHelper.SetAssetBundleSpriteSingle(uiImage, iconPath)
	if not IsNil(uiImage) then
		BundleManager:SetAssetBundleSprite(uiImage, iconPath, "");
	end
end

-- 选择图集中的单图
-- @example UIHelper.SetAssetBundleSpriteAtlas(m_imageIcon, LuaBundlePathString.Atlas_ItemIconPath, "item_1")
function  UIHelper.SetAssetBundleSpriteAtlas(uiImage, iconPath, iconName)
	if not IsNil(uiImage) then
		BundleManager:SetAssetBundleSprite(uiImage, iconPath, iconName);
	end
end
