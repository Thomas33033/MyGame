local Render = {}

function Render:Awake()

end

function Render:Init(tf)
    self.transform = tf
    self.gameObject = tf.gameObject
    self.name = tf.name
    self:Awake()
    UI.FindRenderUI(self,self.transform)
    self:StartInit()
end

function Render:SetUIComponent(child)

end

function Render:StartInit()

end

function Render:OnDestroy()
    --清空资源引用计数器
end

--加载对象，增加计数器便于资源释放
function Render:LoadGameObject(name,node)
    return LoadTools.LoadGameObject(self.path,name,node)
end

--加载对象，增加计数器便于资源释放
function Render:LoadSprite(atlasName,iconName)
   return LoadTools.LoadSprite(atlasName, iconName);
end

return Render
