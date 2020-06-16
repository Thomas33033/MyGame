--主入口函数。从这里开始lua逻辑
require "3rd/class/class"
require "3rd/DebugPrint/debugPrint"
require "logic/Common/LuaTools"
require "logic/Common/ListView"
require "logic/Common/LogicTools"
require "logic/Common/LuaTools"
require "logic/Common/CtrlBase"
require "logic/Common/UIHelper"

require "logic/PB_MANIFEST"
require "logic/PROTO_ENUM"
require "logic/PROTO_MSG"
require "logic/Logic_Config"

require "Logic/Common/SysTimer"
require "Logic/Common/UIHelper"
require "Logic/Common/UIBase"
require "Logic/Common/ProxyBase"
--require "Logic/LuaCsharpRouter";

Vector2 = UnityEngine.Vector2;
Quaternion = UnityEngine.Quaternion;
GameObject = UnityEngine.GameObject;
PlayerPrefs = UnityEngine.PlayerPrefs;


require "Logic/Managers/LuaNetManager"
require "Logic/Managers/LuaUIManager"
require "Logic/Managers/DataManager"
--require "Logic/Managers/LuaGameManager"

print("init")
Main = {}

function Main.Awake()
	--游戏初始化lua模块;
	--注册pb列表;
	Network.RegisterPbs();
	--LuaNetManager.RegisterListener()
	--注册逻辑控制器
	DataManager.RegisterListener();


end

function Main.Start()
	LuaUIManager.ShowUI("UI_Mail")
end

--timeDetail:以秒为单位
function Main.Update(timeDetail)

end


--场景切换通知
function Main.OnLevelWasLoaded(level)
	collectgarbage("collect");
	Time.timeSinceLevelLoad = 0
end