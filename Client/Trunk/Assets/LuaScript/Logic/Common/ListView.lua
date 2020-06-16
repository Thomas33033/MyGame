-- 列表控件
ListView = class("ListView")

function ListView:ctor(args)
    --数据类
    dataList = nil
    -- 单位类
    viewItem = nil
    --滑动列表类
    scrollRect = nil
    --cell对象和luaViewItem关联映射
    objToLuaMap = {}

end

function ListView:init(viewItem,prefab,scrollRect,dataList)
    self.viewItem = viewItem
    self.objToLuaMap = { }
    self.dataList = dataList
    self.scrollRect = scrollRect:GetComponent("LoopVerticalScrollRect")
    --self.scrollRect.prefabSource.prefabGameobj = prefab.gameObject
    self.scrollRect.totalCount = #dataList;
    self.scrollRect:SetLuaScrollCellChangeEvent(function(transObj,objId,index)
        self:OnRefreshData(transObj,objId,index)
    end);
    self.scrollRect:RefillCells();
end

function ListView:ResetScroll( ... )
    -- body
    if self.scrollRect and self.scrollRect.content then
        local orix = self.scrollRect.content:GetComponent("RectTransform").anchoredPosition.x
        self.oriY = self.oriY or 0
        self.scrollRect.content:GetComponent("RectTransform").anchoredPosition = Vector2.New(orix , self.oriY)
    end
end

function ListView:SetYScrollRect(posY)
    if self.scrollRect and self.scrollRect.content then
        local orix = self.scrollRect.content:GetComponent("RectTransform").anchoredPosition.x
        self.scrollRect.content:GetComponent("RectTransform").anchoredPosition = Vector2.New(orix , posY)
    end
end

function ListView:OnRefreshData(transObj,objId,index)
    if self.objToLuaMap[objId] == nil then
        local viewItem = self.viewItem.New()
        viewItem:CreateUI(transObj);
        self.objToLuaMap[objId] = viewItem;
    end
    local itemData = self.dataList[index+1]
    if itemData ~= nil then
        self.objToLuaMap[objId]:SetData(itemData)
    end
end



function ListView:Dispose()
   for i=2,#self.itemArr do
       local item = self.itemArr[i];
       if not IsNil(item) then
            -- GameObject.Destroy(item.gameObject);
            item.gameObject:SetActive(false);
       end
       -- self.itemArr[i] = nil;
   end

   -- self.itemArr = {};
end

-- endregion
ListViewItem = class("ListViewItem")

function ListViewItem:ctor()
    self.index = nil
    self.gameObject = nil
end

function ListViewItem:CreateUI(gameObject)
    self.gameObject = gameObject
    self:Awake(gameObject)
end

function ListViewItem:SetData(data)

end

function ListViewItem:SetActive(bShow)
    if not IsNil(self.gameObject) then
        self.gameObject:SetActive(bShow)
    end
end

function ListViewItem:OnDestroy()

end