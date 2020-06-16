--- 事件中心
require "Logic/Utility";

GyNotify = {
    m_tbClientNotifys = {},
};

----------------------------------------以下协议号定义部分----------------------------------------

------ 客户端内部通信协议号;
-- @table CLIENT_NOTIFI_ID
CLIENT_NOTIFI_ID =
{
    MIN = 100000;

    -------------- C#中传递过来事件（id勿动100000-110000该区段保留）
    C_VIRTURE_KEY_EVENT = 100039;  --键盘按键事件
    C_SOCKET_EXCEPTION = 100042;--网络异常;
    C_UI_OPERATE_MESSAGE = 100086;--有界面开关的消息;
    C_GPS_INFO_ABOUT = 100112;  --GPS
    C_ON_APPLICATION_PAUSE = 100122;        --切换到后台
    C_ON_APPLICATION_PAUSE_TIME = 100123;   --切换到后台的时间
    C_SCENE_LOADING_PROGRESS = 100157;--C#传过来的加载进度;0-1;
    C_SCENE_LOAD_FINISHED = 100158;--C#传过来加载完成;
    C_ONAPPLICATIONFOCUS = 100215;--从前后台切换;
    C_FORNETWORK = 100220;--供网络层使用;
    C_LIMIT_FRAME_RATE = 100221;--限帧中; 

    
    -------------账号相关----------------
    LOGIN_REQ = 110001; --请求登录账号;
    LOGIN_SUCCESS = 110002;--登录成功;
    SELECT_ROLE_SUCCESS = 110003; --选择角色成功 
    CREATE_ROLE_SUCCESS = 110004; --创建角色
    VERSION_CHECK_SERVER = 110005; --登录的时候版本检测;
    OFF_LINE_HANDLE = 110006; --断线了，给一个事件;
    SWITH_ACOUNT_LOGIN = 110017; --切换账号，重新登录;
    ACCOUNT_UPDATE = 110018;        --账号信息更新
    SDK_LOGIN_RET_WX = 110019;      --微信登录返回
    SDK_LOGIN_RET_QQ = 110020;      --QQ登录返回
    SDK_LOGIN_RET_GUEST = 110021;   --游戏登录返回
    
    --------------加载相关-----------------
    CHANGE_SCENE = 120002;--切换场景;
    SCENE_LOADING_PROGRESS = 120003;   --场景加载进度
    LOADING_ALL_END = 120004; --LOADING结束
    LOADING_SHOW_FINISH_SPLASH = 120005; --LOADING结束展示镜头出现


    ---------------战斗相关事件---------------------------------
    BATTE_RECEIVE_GAME_BEGIN = 200001; --收到战斗开始数据
    BATTE_RECEIVE_ROOM_INFO = 200002; --收到房间数据
    BATTLE_FIGHT_PREPARE_OK = 200003;--战斗准备阶段结束;
    BATTLE_FIGHT_ENDING = 200004;--战斗表演结束;  (特殊c#使用id勿动)
    BATTLE_FIGHT_RESULT_ENDING = 200005;    --战斗结算表现完
    BATTLE_SHOW_DAMAGE = 200006; --战斗显示伤害; (特殊c#使用id勿动)
    BATTLE_SHOW_DODGE = 200007; --战斗显示闪避; (特殊c#使用id勿动)
    BATTLE_COUNT_DOWN = 200008; --战斗狂暴倒计时;(arg1:time/ms)
    BATTLE_RAMPAGE_STAGE = 200009; --战斗狂暴阶段;
    BATTLE_RECEIVE_FAST_BATTLE_DATA = 200010; --收到快速战斗数据;
    BATTLE_RECEIVE_FAST_BATTLE_END = 200011; --快速战斗播放结束;
    BATTE_UPDATE_ROOM_STATUS = 200012; --房间状态更新;
    BATTLE_BREAK_SKILL = 200013; --战斗显示打断
    
    ---------------通用功能事件---------------------
    SHOW_BOX = 300001;--显示二次确认框;
    UIOPERATION_SWITCH_OPTUI = 310001; --主界面隐藏或显示操作UI;
    UIOPERATION_LIFTBOTTOMUI = 310002; --提升主界面底部;
    UIOPERATION_SHOW_BOSS_COMING = 310003; --主界面Boss来袭(特殊c#使用id勿动)
    UIOPERATION_SHOW_TEAM_TIPS = 310004;   --主界面队伍提示面板
    UIOPERATION_REFRESH_MINE_TEAM = 310005;   --刷新我的队伍
    UIOPERATION_SHOW_GET_EXP = 310006; --展示获得经验贝壳{exp = 100, beike = 1}
    UIOPERATION_REFRESH_MINI_MAP = 310007;--刷新小地图
    UIOPERATION_REFRESH_WORLD_MAP = 310008;--刷新大地图
    UIOPERATION_LIFTUPPERUI = 310009; --提升主界面顶部;
    UIOPERATION_BACK_TO_ROLE_INFO_CLICK = 310010;--属性加点/强化装备界面返回点击事件
    UIOPERATION_SHOW_FAST_BATTLE_WAIT = 310011;--展示快速战斗等待中
    UIOPERATION_HIDE_FAST_BATTLE_WAIT = 310012;--隐藏快速战斗等待中
    UIOPERATION_SHOW_FUNCTION_DES_PAGE = 310013;--打开功能描述界面
    UIOPERATION_REFRESH_FAST_BATTLE_UI = 310014;--收到快速战斗剩余次数

    ---------------组队相关事件---------------------
	UITEAMINFO_SETCHILD = 320001; --组队界面设置子界面父节点
    UITEAMINFO_ROOM_MATCH_LIST = 320002; --组队界面设置子界面父节点
    UITEAMINFO_ROOM_EXPLORER_LIST = 320003; --冒险者大厅中的玩家列表
    UITEAMINFO_TIPS = 320004; --刷新组队提示界面信息
    UITEAMINFO_APPLY_LIST = 320005; --队伍组队申请
    UITEAMINFO_APPLY_TIPS = 320006; --队伍组队申请提示，打开界面时直接发送
    UITEAMINFO_MESSAGE_LIST = 320007; --队伍消息列表
    UITEAMINFO_CLICK_MINEMESSAGE_ITEM = 320008; --我的队伍列表点击事件
    UITEAMINFO_OPEN_SET_TEAMMATE_REFRESH_SKILL = 320009; --修改队友属性界面-刷新队友技能
    
    --营地地图消息
    UICAMPSITE_SWITCH_VISIBLE_STATE = 330001; --营地地图是否显示

    ---------------GM指令相关事件---------------------
    GM_BTN_CLICK= 400001;--GM点击事件;

    ---------------道具相关事件---------------------
    BAG_SHOW_ITEM_TIPS_BY_TABLE_ID = 500001;--通过表格配置id显示;
    BAG_SHOW_ITEM_TIPS_BY_DATA = 500002;--通过道具数据显示;
    BAG_GET_ITEM_RES = 500003; --获得道具
    BAG_UPDATE_DEPOTS = 500004; --刷新道具

    ---------------主角信息相关事件---------------------
    ROLE_INFO_CHANGE = 550001;--角色PlayerFullInfo基础信息刷新
    ROLE_EXP_CASH_GOLD_CHANGE = 550002;--角色经验、金币、元宝刷新
    ROLE_PRO_CHANGE = 550003; --角色属性变化
    ROLE_FIGHT_CHANGE = 550004;--角色战力变化

    ---------------装备穿戴相关事件---------------------
    EQUIP_SHOW_EQUILIST_BY_SLOT = 600001;--通过装备部位显示对应部位可穿戴的装备列表
    EQUIP_OTHER_ROLEINFO_ENTER_SHOW = 600002; --显示其他玩家所有信息入口
    EQUIP_ROLE_EQUIPLIST_UPDATE = 600003; --角色属性界面刷新装备列表
    EQUIP_ROLE_POINT_UPDATE = 600004; --角色属性加点界面刷新(属性加点成功)
    EQUIP_INFO_SHOW = 600005;--装备信息详情显示
    EQUIP_UPGRADE_SHOW = 600006;--打开装备附魔/强化界面
    EQUIP_UPGRADE_SUCCESS = 600007;--打开装备附魔/强化成功
    EQUIP_QIANGHUA_ALL_SUCCESS = 600008; --一键强化成功
    EQUIP_OTHER_ROLEINFO_SHOW = 600009; --显示其他玩家模型信息
    EQUIP_OTHER_ROLEPRO_SHOW = 600010; --显示其他玩家装备及属性信息
    EQUIP_OTHER_ROLEPET_SHOW = 600011; --显示其他玩家宠物信息
    EQUIP_BUILDINFO_REFRESH = 600012; --装备打造刷新
    EQUIP_ROLE_RESULT_POINT_CHANGE = 600013; --角色剩余属性加点变化

    --------------技能相关事件--------------------------
    SKILL_ROLE_SKILLLIST_UPDATE = 700001;   --人物技能列表刷新
    SKILL_ROLE_OPEN_REPLACE_SKILL = 700002; --打开替换技能界面
    SKILL_ROLE_OPEN_LEVELUP_SKILL = 700003; --打开升级技能界面
    SKILL_ROLE_RERESH_SKILL = 700003;      --刷新技能
    SKILL_ROLE_HAS_NEW_SKILL = 700004;      --背包新道具红点
    SKILL_ROLE_CAN_EQUIP_SKILL = 700005;      --技能栏可以装备红点


    ---------------聊天指令相关事件---------------------
    CHAT_RECEIVE_CHAT_MSG = 800001;--收到聊天消息事件;
    CHAT_UPDATE_CHANNEL = 800002;--聊天频道改变;
    CHAT_START_PRIVATE_CHAT = 800003;--发起私聊 {name, uuid}
    CHAT_CLICK_OP_CHAT_ICON = 800004;--点击人物Icon弹出操作界面{name, uuid}
    CHAT_CLICK_PRIVATECHAT_GROUP = 800005;--点击私聊界面Group{channelType, channelID}
    CHAT_OPEN_TEAM_GROUP = 800006;--点击私聊界面,切换到组队频道
    CHAT_BATTLE_INFO_MSG = 800007;--聊天战报信息{attackerName, targetName, attackerTeamID, targerTeamID, hurtValue}

    ---------------邮件相关---------------------
    MAIL_DATA_DUPDATED = 900001; --邮件数据更新
    MAIL_CONTENT = 900002; --传递邮件内容用于显示界面

    ---------------宠物界面相关事件---------------------
    PET_ITEM_CLICK_MSG = 650001;--宠物界面item点击事件;
    PET_LIST_REFRESH = 650002;--宠物界面宠物列表上线初始化/宠物解锁或锁成功/宠物放生成功
    PET_FIGHT_OR_SLEEP = 650003;--宠物出战或休息成功
    PET_UPGRADE_SUCCESS = 650004;--宠物研究所升级成功
    PET_HECHENG_SUCCESS = 650005;--宠物碎片合成成功
    PET_SUIPIAN_ITEM_CLICK = 650006;--宠物碎片合成界面item点击事件;


};

function GeneratorNotifyId()
    local idArr = {};
    local index = 100000;
    for k,v in pairs(CLIENT_NOTIFI_ID) do
        CLIENT_NOTIFI_ID[k] = k;
        index = index + 1;
    end
end

function CheckNotifyIdValid()
    local idArr = {};
    for k,v in pairs(CLIENT_NOTIFI_ID) do
        if idArr[v] == nil then
            idArr[v] = true;
        else
            error("CLIENT_NOTIFI_ID id 重复:", k, v);
        end
    end
end

GeneratorNotifyId();
CheckNotifyIdValid();


---------------------------------------------------------------
---------------------------------------------------------------
---------------------------------------------------------------
---------------------------------------------------------------
--------------------客户端消息逻辑部分 开始--------------------

function GyNotify:RegisterNotify(notifyID, callback, observer)
    local currNotify = self.m_tbClientNotifys[notifyID];
    if (not currNotify) then
        currNotify = {};
        self.m_tbClientNotifys[notifyID] = currNotify;
    else
        for i,v in pairs(currNotify) do
            if (v.callback == callback) and (v.observer == observer) then
                return;
            end
        end
    end

    if type(observer) == "table" then
        table.insert(currNotify, {observer = observer, callback = callback});
    end
end

function GyNotify:SendNotification(notifyID, notification)
    local currNotify = self.m_tbClientNotifys[notifyID];
    if currNotify then
        notification = notification or {};
        notification.notifyID = notifyID;
        for i,v in pairs(currNotify) do
            v.callback(v.observer, notification);
        end
    end
end

function GyNotify:UnRegisterNotify(notifyID, observer)
	local currNotify = self.m_tbClientNotifys[notifyID];
	if (not currNotify) then
		return;
	end

	if (not observer) then
		self.m_tbClientNotifys[notifyID] = nil;
	else
        for i,v in pairs(currNotify) do
            if (v.observer == observer) then
                table.remove(currNotify, i);
            end
        end
	end
end

function GyNotify:HasNotify(notifyID)
    local currNotify = self.m_tbClientNotifys[notifyID];
    if currNotify then
        return true;
    end
    return false;
end
--------------------客户端消息逻辑部分 结束--------------------

