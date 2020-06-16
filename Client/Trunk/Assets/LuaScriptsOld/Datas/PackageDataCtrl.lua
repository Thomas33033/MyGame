SingleBagItem = class ("SingleBagItem" )
function SingleBagItem:ctor()
	self.m_BagItem = nil
	self.m_Cfg = nil
end
---@param item BagItem
function SingleBagItem:init(item)
	self.m_BagItem = item
	self.m_Cfg = cfg_item[item.Id]
end

---@param singleItem SingleBagItem
---@param item BagItem
function SingleBagItem.UpdateOrCreateItem(singleItem, item)
	if singleItem == nil then
		singleItem = SingleBagItem.New()
	end
	singleItem:init(item)
	return singleItem;
end

PackageGroup = class("PackGroup")
function PackageGroup:ctor()
	self.m_PackagetType = nil
	self.m_SingleItemMap = {}
end

function PackageGroup:SetType(type)
	self.m_PackagetType = type
end

---@param singleItem SingleBagItem
function PackageGroup:UpdateSingleItem(singleItem)
	if self.m_SingleItemMap == nil then
		self.m_SingleItemMap = {}
	end
	self.m_SingleItemMap[singleItem.bagItem.Uuid] = singleItem
end


---@param bagItem BagItem
function PackageGroup:UpdateBagItem(bagItem)
	if self.m_SingleItemMap == nil then
		self.m_SingleItemMap = {}
	end
	self.m_SingleItemMap[bagItem.Uuid] = SingleBagItem.UpdateOrCreateItem(self.m_SingleItemMap[bagItem.Uuid], bagItem)
end

---@return SingleBagItem
---param uuid string
function PackageGroup:GetItemBayUUID(uuid)
	if self.m_SingleItemMap == nil then
		return nil
	end
	return this.singleItemMap[uuid]
end

function PackageGroup:GetAllItems()
	return self.m_SingleItemMap;
end


G_BagDataCtrl = nil
BagDataCtrl = class("BagDataCtrl", DataCtrlBase)



function BagDataCtrl:ctor()
	self.m_ItemNumMap = {}
	self.m_PackageGroupMap = {}
end


function BagDataCtrl:Awake()
	G_MainUserDataCtrl = self
	self:RegisterMessages()
	self.super.Awake(self);
end

function BagDataCtrl:RegisterMessages()
	NetworkManager.RegisterProtoMsg("SC_UpdateBagItems", function(msgData) self:SC_UpdateBagItems(msgData) end)
end

--------------------接收服务器消息----------------------
---@param msgData SC_UpdateBagItems
function BagDataCtrl: SC_UpdateBagItems(msgData)
	if self.m_PackageGroupMap == nil then
		self.m_PackageGroupMap = {}
	end
	for i = 0, i < msgData.Items.Count(), 1 do
		---@type BagItems
		local bagItems = msgData.Items[i]
		local packageGroup = self.GetOrCreatePackageGroup(bagItems.type)
		for v in bagItems.Items do
			---@type BagItem
			local item = bagItems[v]

			---@type SingleBagItem
			local replacedItem = packageGroup:GetItemBayUUID(item.Uuid)
			if replacedItem ~= nil then
				self:UpdateItemNumByBaseID(replacedItem.Id, item.num - replacedItem.num)
			else
				self:UpdateItemNumByBaseID(replacedItem.Id, item.num )
			end

			packageGroup:UpdateBagItem(item)
		end
	end
end


--------------------向服务器发送消息----------------------


--------------------逻辑相关----------------------
function BagDataCtrl:Update()
	self.super.Update(self)
end

function BagDataCtrl:OnReconnect()
	self.super.OnReconnect(self)
end

function BagDataCtrl:OnDestory()
	self.super.OnDestroy(self)
end

---@return PackageGroup
function BagDataCtrl:GetOrCreatePackageGroup(type)
	if self.m_PackageGroupMap[type] == nil then
		local group = PackageGroup.New()
		group.SetType(type)
		self.m_PackageGroupMap[type] = group
	end
	return self.m_PackageGroupMap[type]
end

---@return SingleBagItem
function BagDataCtrl:GetSingleItemByUUID(uuid)
	for type in self.m_PackageGroupMap do
		---@type PackageGroup
		local packageGroup = self.m_PackageGroupMap[type]
		---@type SingleBagItem
		local findedItem  = packageGroup:GetItemBayUUID(uuid)
		if findedItem ~= nil then
			return findedItem
		end
	end
	return nil
end

function BagDataCtrl:UpdateItemNumByBaseID(id, num)
	if self.m_ItemNumMap[id] == nil then
		self.m_ItemNumMap[id] = num
	else
		self.m_ItemNumMap[id] = math.max(0,  self.m_ItemNumMap[id] + num)
	end
end

function BagDataCtrl:GetItemNumByBaseID(id)
	if self.m_ItemNumMap[id] == nil then
		return 0
	end
	return math.max(self.m_ItemNumMap[id], 0)
end



