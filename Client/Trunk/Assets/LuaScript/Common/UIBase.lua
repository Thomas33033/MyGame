--界面基类
--界面卸载只能 通过lua调用DoClose方法销毁lua内存，单独删除界面不会通知lua
UIBase = {}

function UIBase:Create(path,name,layer)
	self.path = path
	self.name = name
	self.layer = layer
end

function UIBase:Awake()

end

function UIBase:Update()
	
end

function UIBase:OnEnable()

end

function UIBase:SetUIComponent(child)

end

function UIBase:ButtonClickHandler(btn)

end

function UIBase:DoClose()
	Event.Call("UIClosed",self.name)
	if AutoUnloadUIMap[self.path] == 1 then
		LoadTools.UnLoadAssetsBundle(self.path)
	end

	self:OnClose()
	UnityEngine.GameObject.Destroy(self.gameObject)
	self:Clear()
end

function UIBase:Clear()
	if self.dicEvent ~= nil then
		for k,v in pairs(self.dicEvent) do
			Event.Remove(k,self._eventCallback)
		end
	end
	UI.ClearUITable(self)
end

--加载图片资源，增加计数器便于资源释放
function UIBase:LoadSprite(name)
	tempIndex = tempIndex + 1
	self["tAeBmCp"..tempIndex] = LoadTools.LoadSpriteFromBundle(self.path,name)
	return self["tAeBmCp"..tempIndex]
end

--添加时间监听
function UIBase:AddListener(type,callback)
	if self.dicEvent == nil then
		self.dicEvent = {}
	end
	self.dicEvent[type] = callback
	if self._eventCallback == nil then
		self._eventCallback = function (type,...)
			if self.dicEvent[type] ~= nil then
				self.dicEvent[type](self,type,...)
			end
		end
	end

	Event.Add(type,self._eventCallback)
end

--加载对象，增加计数器便于资源释放
function UIBase:LoadGameObject(name,node)
	return LoadTools.LoadGameObject(self.path,name,node)
end

--加载对象，增加计数器便于资源释放
function UIBase:LoadSprite(atlasName,iconName)
	LoadTools.LoadSprite(atlasName, iconName);
end

