--主入口函数。从这里开始lua逻辑
require "3rd/Class/class"

require "Common/LuaTools"
require "Common/LogicTools"
require "Common/GameEnum"
require "Managers/LuaNetManager"
require "Managers/UIManager"
require "Managers/CtrlManager"
require "Managers/space3d"

require "Common/Utility"

Quaternion = UnityEngine.Quaternion;
GameObject = UnityEngine.GameObject;
PlayerPrefs = UnityEngine.PlayerPrefs;

require "Space/Home/Home"

Main = {}
function Main.Awake()
	--游戏初始化lua模块;
	--注册pb列表;
	Network.RegisterPbs();
	--LuaNetManager.RegisterListener()
	--注册逻辑控制器
	CtrlManager.RegisterListener();

	Main.Start()
end

function Main.Start()
	--切换场景
	Home.Create()
end

--timeDetail:以秒为单位
function Main.Update(timeDetail)

end

--场景切换通知
function Main.OnLevelWasLoaded(level)
	collectgarbage("collect");
	Time.timeSinceLevelLoad = 0
end