UIHelper = {}
local this = UIHelper

--设置列表渲染接口
function UIHelper.SetListItemRender(list,func)
	LuaPanelBase.SetListItemRender(list, func)
end

--获取图片组件
function UIHelper.GetImage(GView,name)
	local obj = this.GetObject(GView,name)
	if not obj then
		return
	end
	return obj.asImage
end

--获取图片加载器
function UIHelper.GetLoader(GView,name)
	local obj = this.GetObject(GView,name)
	if not obj then
		return
	end
	return obj.asLoader
end

--获取文本组件GTextField
function UIHelper.GetTextField(GView,name)
	local obj = this.GetObject(GView,name)
	if not obj then
		return
	end
	return obj.asTextField
end

--获取列表组件GList
function UIHelper.GetList(GView,name)
	local obj = this.GetObject(GView,name)
	if not obj then
		return
	end
	return obj.asList
end

--获取按钮组件
function UIHelper.GetButton(GView,name)
	local obj = this.GetObject(GView,name)
	if not obj then
		return
	end
	return obj.asButton
end

-- 检查界面是否存在
function UIHelper.GetObject(GView,objName)

	if type(objName) ~= 'string' then
		error("警告： objName:" .. objName .. " is not string。")
		return nil
	end
		
	local obj = GView:GetChild(objName)
	if not obj then
		error(objName .. "不存在请检查，界面名称:")
	end
	return obj
end