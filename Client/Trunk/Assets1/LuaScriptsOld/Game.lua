--游戏启动类

require "Global/Const"
require "Common/Debug"
require "Common/class"
require "View/UIBase/UIPanelBase"
require "Manager/DataManager"
require "Manager/NetWorkManager"
require "Manager/UIManager"
require "FairyGUI"

Game = {};

local this = Game;



function InitLuaGame()
--初始化本地配置，比如ProtoPb文件，资源配置等
	NetworkManager.LoadProtoPB();
    print("InitLuaGame Call Success");
end


function Game.Awake()
	print("游戏启动...")

	--加载协议资源
	--NetworkManager.LoadProtoPB();
	
	--初始化控制器
	--UIManager.New()
	--DataManager.New();

	UIPackage.AddPackage("GUISkin/Common")
	UIPackage.AddPackage("GUISkin/UI_login")
	local view = UIPackage.CreateObject("UI_login", "UI_login")
	GRoot.inst:AddChild(view)
	
end

function Game.Update(timeDetail)
	--print("---update--",timeDetail)
end

function Game.OnDestroy()

end





