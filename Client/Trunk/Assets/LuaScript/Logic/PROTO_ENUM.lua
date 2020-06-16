
-- 存放客户端枚举

ELoginAccountType =
{
    -- 输入
    Input = 0;
    -- 游客
    Guest = 1;
    -- 微信
    Wx = 2;
    -- QQ
    QQ = 3;
    -- 无账号
    None = 404;
}

-- 账号登陆错误码
EAccountLoginCodes = 
{
	-- 成功
    ACCOUNTLOGIN_SEC = 0;
	-- 用户名或密码错误
	ACCOUNTLOGIN_ERRORACCOUNT = 1;
	-- 微信登录授权失败
	ACCOUNTLOGIN_ERRORWXAUTH = 2;
}

EFighterItemType = 
{
    --玩家
    UNDEFINE = 0;
    --玩家
    PLAYER = 1;
    --NPC
    NPC = 2;
}

--// 胜利方枚举
EWinnerEnum =
{
    --// 没有胜利者
    NONE = 0,
    --// 挑战方
    ATTACKER = 1,
    --// 防守方
    DEFENDER = 2,
}

--身体部位
EAvatarPartType = 
{
    Body = 0,
    Weapon = 1,
}


EGenderType = {
    Man = 1,
    Woman = 2
}
--品质枚举
ERareType = {
    --普通
    White = 1;
    --优秀
    Green = 2;
    --稀有
    Blue = 3;
    --史诗
    Pruple = 4;
    --传说
    Gold = 5;
    --神话
    DarkGold = 6;
}

EProfessionType = {
    Job1 = 1,
    Job2 = 2,
    Job3 = 3,
    Job4 = 4,
}

--服务器繁忙状态
EServerFlowType = {
    --流畅 
    ZONE_FLOW_EASY = 1,
    --繁忙
    ZONE_FLOW_BUSY = 2,
    --爆满
    ZONE_FLOW_HOT = 3,
}

EDepotClasss = {
    -- 未知
    DEPOTCLASSS_UNDEFINE = 0;
    -- 货币
    EPOTCLASSS_COIN = 1;
    -- 装备
    DEPOTCLASSS_EQUIP = 2;
    --食物
    DEPOTCLASSS_FOOD = 3;
    --宠物
    DEPOTCLASSS_PET = 4;
    --宝石
    DEPOTCLASSS_GEM = 5;
    --其他
    DEPOTCLASSS_OTHER = 6;
    --技能
    DEPOTCLASSS_SKILL = 7;
}
--现金id
ECashID =
{
    E_CASH = 100,
    E_GOLD = 101,
}

ETeamPageType = {
    --队伍消息
    TEAM_MESSAGE = 1,
    --队伍
    TEAM_MAIN = 2,
    --队伍公告
    TEAM_NOTICE = 3,
    --冒险者大厅
    TEAM_HALL = 4,
}

--队伍匹配模式
ERoomMatchTypes = {
	--未定义
	ROOMMATCH_UNDEFINE = 0,
	--挂机队
	ROOMMATCH_BUDDHA = 1,
	--推图队
	ROOMMATCH_PUSHON = 2,
}

--队伍加入权限
EJoinAuths = {
	--任何人可直接加入
	EVERYONE = 0,
	--需要验证
	NEEDVERIFI = 1,
}

--队伍加入类型
EJoinTypes = {
    --快速组队
    AUTO = 0,
    --创建玩家自建队伍
    CREATE = 1,
    --加入玩家自建队伍
    ADDIN = 2,
}

-- 聊天频道类型枚举
EChatChannelTypes ={
        -- 无效
    CHATCHANNELTYPE_UNDEFINE = 0,
    -- 公共频道
    CHATCHANNELTYPE_PUBLIC = 1;
    -- 个人私聊频道
    CHATCHANNELTYPE_PERSONAL = 2;
    -- 房间频道
    CHATCHANNELTYPE_ROOM = 3;
    -- 家族-部落 频道
    CHATCHANNELTYPE_GUILD = 4;
}
--聊天频道状态
EChannelStates = {
        -- 无效
    NONE = 0;
    -- 畅通
    GREEN = 1;
    -- 拥挤
    YELLOW = 2;
    -- 爆满
    RED = 3;
}

LuaEntityType = 
{
    UNDEFINE = 0;   --玩家
    PLAYER = 1; --玩家
    NPC = 2;    --npc
    PET = 3;    --宠物
    UIPLAYER = 254;    --UI角色
    NULL = 255;
}

--战斗类型
LuaBattleTypes = 
{
    UNDEFINE = 0;   --// 错误 未定义
    LEVEL_MONSTER = 1;  --// 闯关 小怪战
    LEVEL_LITTLEBOSS = 2;   --  // 闯关 小BOSS战
}

--UI
CLIENT_UI_COLOR = 
{
    COLOR_HIGHTLIGHT = Color.white;
    COLOR_GRAY       = Color.New(0.78,0.78,0.78,1);
    COLOR_ENABLE     = Color.New(171 / 255, 205 / 255, 239 / 255, 1);
    COLOR_NAME_ONLINE = Color.white;--Color.New(16 / 255, 128 / 255 ,123 / 255,1);
    COLOR_ONLINE = Color.New(66 / 255, 255 / 255 ,41 / 255,1);
    COLOR_OFFLINE = Color.New(66 / 255, 255 / 255 ,41 / 255,1);

    COLOR_GREEN = Color.green;
    COLOR_RED = Color.red;
}


NET_TAG =
{
    REQ_HEART = 1; --请求心跳包;
    OFF_LINE = 2; --断线检测;
    SHOW_WAITING = 3; --显示等待圈;
}


--房间队伍消息类型
ERoomNewsTypes = {
	--无效
	ROOMNEWS_UNDEFINE = 0;
	--加入了房间
	ROOMNEWS_JOINROOM = 1;
	--加入了房间，切换挂机地点
	ROOMNEWS_JOINROOMCHANGE = 2;
	--挑战BOSS成功
	ROOMNEWS_CHALLENGEBOSSSEC = 3;
    -- 快速战斗
    ROOMNEWS_FASTBATTLE = 4;
    --成员退出了房间
    ROOMNEWS_MEMBERQUIT = 5;
}

--心跳类型
EHeartbeatTypes = {
	--无效
	HEARTBEAT_UNDEFINE = 0;
	--请求
	HEARTBEAT_PING = 1;
	--响应
	HEARTBEAT_PONG = 2;
}


ERoomMemberOptions = {
    --无效
    UNDEFINE = 0;
    --踢出队伍
    KICKOUT = 1;
    --转让队长
    SETLEADER = 2;
    --设为副队长
    SETVICELEADER = 3;
    --顶替队长
    REPLACELEADER = 4;
}

EItemType = {
    --货币
    CURRENCY = 1,
    --装备
    EQUIP = 3,
    --内丹
    INNER_DAN = 3,
    --宠物
    PET = 4,
    --宝石
    GEM = 5,
    --其他
    OTHER = 6,
    --技能
    SKILL = 7
}

--技能页
ESkillPages = {
    --无效
    SKILLPAGE_UNDEFINE = 0,
    --狩猎技能页
    SKILLPAGE_FIGHTING = 1,
    --挂机技能页
    SKILLPAGE_GUAJI = 2,
}


--房间队伍消息统计数据类型
ERoomStatisticalTypes = {
    --无效
    ROOMSTATISTICAL_UNDEFINE = 0;
    --关卡推进数量
    ROOMSTATISTICAL_MAPPUSHNUM = 1;
    --快速战斗
    ROOMSTATISTICAL_FASTBATTLE = 3;
}
----------------------------------
-------------------------------------------------------------------------------------------------------------
--用于PlayerPrefs的key;
PlayerPrefsStrings = 
{
    LAST_ACCOUNT_ID = "LAST_ACCOUNT_ID"; --上次登陆Id
}
