--注意：这个文件由工具自动生成,手动修改可能会被覆盖;


------------------------本地配置数据表------------------------;
require "Configs/cfg_item";
require "Configs/cfg_npc";
require "Configs/cfg_npc_attr";
require "Configs/cfg_ui";
------------------------本地配置数据表------------------------;


------------------------所有的通信协议文件列表------------------------;
require "UI/Mail/Controller/MailCtrl";
require "UI/Main/Controller/MainCtrl";
------------------------所有的通信协议文件列表------------------------;


-----------------------管理模块控制器--------------------------------
ModelCtrlArray = {}
ModelCtrlArray[0] = MailCtrl
ModelCtrlArray[1] = MainCtrl
