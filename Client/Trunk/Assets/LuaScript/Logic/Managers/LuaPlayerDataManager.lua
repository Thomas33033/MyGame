require "Logic/LocalStorage"

--玩家数据管理
LuaPlayerDataManager = {};
local self = LuaPlayerDataManager

LuaPlayerDataManager.m_mainUserData = nil;
LuaPlayerDataManager.LoginSuccess = false

LuaPlayerDataManager.m_playerCoin = nil

--玩家属性管理;
Player = 
{
	IP = "",
	PORT = 10001,
	DOMAIN_FLAG = 1,	
	SERVER_ID = nil,
}

-- 账号数据
-- @param p_playerFullInfo:PlayerFullInfo
function LuaPlayerDataManager.SetAccountData(p_playerFullInfo)
	self.m_mainUserData = p_playerFullInfo;
	self.SetRoleSkills(self.m_mainUserData.Skill)
end

function LuaPlayerDataManager.SetPlayerCoin(msgData)
	self.m_playerCoin = msgData
end

--主角当前经验
function LuaPlayerDataManager.GetRoleExp()
	if self.m_playerCoin==nil then
		return 0
	end
	return tonumber(self.m_playerCoin.Exp)
end

--主角铜钱
function LuaPlayerDataManager.GetRoleCash()
	if self.m_playerCoin==nil then
		return 0
	end
	return self.m_playerCoin.Cash
end

--主角元宝
function LuaPlayerDataManager.GetRoleGold()
	if self.m_playerCoin==nil then
		return 0
	end
	return self.m_playerCoin.Gold
end

--返回主解的唯一ID
function LuaPlayerDataManager.GetUUID()
	return self.m_mainUserData.BaseInfo.UUID
end

--获取角色名称
function LuaPlayerDataManager.GetRoleName()
	return self.m_mainUserData.BaseInfo.Name
end

--获取角色等级
function LuaPlayerDataManager.GetRoleLevel()
	return self.m_mainUserData.BaseInfo.Level
end

--获取角色性别
function LuaPlayerDataManager.GetRoleSex()
	return self.m_mainUserData.BaseInfo.Gender
end

--获取职业
function LuaPlayerDataManager.GetRoleJob()
	return self.m_mainUserData.BaseInfo.Profession
end

--主角属性值
function LuaPlayerDataManager.GetRoleAttr()
	return self.m_mainUserData.Attr
end

--主角已装备信息
function LuaPlayerDataManager.GetRoleEquips()
	return self.m_mainUserData.EquipInfo.Equips
end

--设置角色技能
function LuaPlayerDataManager.SetRoleSkills(skill)
	self.m_mainUserData.Skill = skill
	self.skillInfoMap = G_SkillRoleProxy:SwitchRoleSkillsToClient(self.m_mainUserData.Skill)
	G_SkillRoleProxy:CalculateSkillRedPoint()
	GyNotify:SendNotification(CLIENT_NOTIFI_ID.BAG_UPDATE_DEPOTS, {})
end

--主角技能信息
function LuaPlayerDataManager.GetRoleSkills()

	if self.skillInfoMap == nil then
		error("错误：技能未初始化")
	end

	return self.skillInfoMap
end

--主角属性点(剩余点数)信息
function LuaPlayerDataManager.GetRoleResultPoint()
	local result = 0;
	if self.m_mainUserData.Point ~= nil and self.m_mainUserData.Point.ENGRY ~= nil then
		result = self.m_mainUserData.Point.ENGRY
	end
	return result
end
--主角已分配属性点数
--STR;--力量
--INT;--智力
--DEX;--敏捷
--CON;--体质
function LuaPlayerDataManager.GetRolePoint()
	local point = {};
	if self.m_mainUserData.Point==nil then
		self.m_mainUserData.Point = {ENGRY = 0, STR = 0, INT = 0, DEX = 0, CON = 0,}
	end
	return self.m_mainUserData.Point
end

--免费重置属性点的剩余次数(todo 后续做成看广告获得重置次数)
function LuaPlayerDataManager.GetResetRolePointCount()
	return 1
end

--主角战斗力
function LuaPlayerDataManager.GetRoleFight()
	return self.m_mainUserData.Fightpoint
end

--主角出战的宠物
function LuaPlayerDataManager.GetRolePet()
	return self.m_mainUserData.Pet
end