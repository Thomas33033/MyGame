--游戏逻辑工具
LogicTools = {}

LogicTools.UIMap = nil




--全局配置表加工数据
LogicTools.cfg_global_entends = nil;

--通过全局配置表字符串key查找配置信息
--@example local cfgGlobal = LogicTools.GetGlobalConfigByKey("SHARE_TITLE");
function LogicTools.GetGlobalConfigByKey(p_key)
    if LogicTools.cfg_global_entends == nil then
        LogicTools.cfg_global_entends = {};
        for k,v in pairs(cfg_global) do
            if v.fileldName then
                if LogicTools.cfg_global_entends[v.fileldName] ~= nil then
                    error("全局配置表变量名重复:", k, v.fileldName);
                else
                    LogicTools.cfg_global_entends[v.fileldName] = v;
                end
            end
        end
    end
    return LogicTools.cfg_global_entends[p_key];
end


--获取角色头像
--type:职业类型 以后统一修改
function LogicTools.SetIconBySex(imageObj, type)
    local iconName = ""
    type = math.floor(type / 100) -100
    if type == EProfessionType.Job1 then
        iconName = "20"
    elseif type == EProfessionType.Job2 then
        iconName = "21"
    elseif type == EProfessionType.Job3 then
        iconName = "22"
    elseif type == EProfessionType.Job4 then
        iconName = "23"
    else
        iconName = "24"
    end

    local path = LuaBundlePathString.SingleImage_HeadIconsPath .."/".. iconName ..".gatecen"
    BundleManager:SetAssetBundleSprite(imageObj, path , iconName)
end


--获取职业图标
--type:职业类型
function LogicTools.SetIconByProfession(imageObj, type, isAI)
    local path = "combine\\ui\\shares\\Common.gatecen"
    local iconName = ""
    type = math.floor(type / 100) -100
    if type == EProfessionType.Job1 then
        iconName = "common-zhiye-201"
    elseif type == EProfessionType.Job2 then
        iconName = "common-zhiye-202"
    elseif type == EProfessionType.Job3 then
        iconName = "common-zhiye-203"
    elseif type == EProfessionType.Job4 then
        iconName = "common-zhiye-204"
    else
        iconName = "common-zhiye-201"
    end


    if isAI == true then
        iconName = "common-zhiye-200"
    end
    BundleManager:SetAssetBundleSprite(imageObj, path, iconName)
end

--设置宠物头像
function LogicTools.SetPetIcon(p_imgIcon,petCfgId)
    local petCfg = cfg_petbase[petCfgId..""];
    if petCfg ~= nil then
        UIHelper.SetAssetBundleSpriteAtlas(p_imgIcon, LuaBundlePathString.Atlas_UIPetSpritePath, petCfg.icon)
        return true
    else
        return false
    end
end


--根据品质获取品质框
function LogicTools.SetQualityKuangByRare(imageObj, rare)
    local path = LuaBundlePathString.Atlas_CommonUISpritePath
    local iconName = ""
    if rare == ERareType.White then
        iconName = KItemQualitykTable[1]
    elseif type == ERareType.Green then
        iconName = KItemQualitykTable[2]
    elseif type == ERareType.Blue then
        iconName = KItemQualitykTable[3]
    elseif type == ERareType.Pruple then
        iconName = KItemQualitykTable[4]
    elseif type == ERareType.Gold then
        iconName = KItemQualitykTable[5]
    elseif type == ERareType.DarkGold then
        iconName = KItemQualitykTable[5]
    else
        iconName = KItemQualitykTable[1]
    end
    BundleManager:SetAssetBundleSprite(imageObj, path, iconName)
end

--根据品质获取品质描述
function LogicTools.SetQualityLabelByRare(imageObj, rare)
    local path = LuaBundlePathString.Atlas_CommonUISpritePath
    local iconName = ""
    if rare == ERareType.White then
        iconName = KItemQualitykTable["quality_1"]
    elseif type == ERareType.Green then
        iconName = KItemQualitykTable["quality_2"]
    elseif type == ERareType.Blue then
        iconName = KItemQualitykTable["quality_3"]
    elseif type == ERareType.Pruple then
        iconName = KItemQualitykTable["quality_4"]
    elseif type == ERareType.Gold then
        iconName = KItemQualitykTable["quality_5"]
    elseif type == ERareType.DarkGold then
        iconName = KItemQualitykTable["quality_5"]
    else
        iconName = KItemQualitykTable["quality_1"]
    end
    BundleManager:SetAssetBundleSprite(imageObj, path, iconName)
end

--根据品质获取技能品质描述图片
function LogicTools.SetSKillLabelByRare(imageObj, rare)
    local path = LuaBundlePathString.Atlas_CommonUISpritePath
    local iconName = ""
    if rare == ERareType.White then
        iconName = "common-pinzhi-106"
    elseif rare == ERareType.Green then
        iconName = "common-pinzhi-105"
    elseif rare == ERareType.Blue then
        iconName = "common-pinzhi-104"
    elseif rare == ERareType.Pruple  then
        iconName = "common-pinzhi-103"
    elseif rare == ERareType.Gold then
        iconName = "common-pinzhi-102"
    elseif rare == ERareType.DarkGold then
        iconName = "common-pinzhi-101"
    else
        iconName = "common-pinzhi-101"
    end
    BundleManager:SetAssetBundleSprite(imageObj, path, iconName)
end


function LogicTools.PlayMapBgSound()
    local roomLevel = BattleProxy.GetCurRoomLevel();
    local cfg = cfg_levelMonster[roomLevel .. ""] or cfg_levelMonster["1"]
    local curMapId = cfg.mapid
    
    for k,v in pairs(cfg_sound) do
        if v.MapId.."" == curMapId.."" then
            LogicTools.PlayBackGround(v.SoundId)
        end
    end
end

-- 关闭背景音乐 不能用 会有bug [LuaException: event:171: Index was out of range. Must be non-negative and less than the size of the collection.]
function LogicTools.StopBackGround()
    -- SoundManger:StopSound(ESoundLayer.Background);
end

function LogicTools.PlayBackGround(id)
    local soundCfg = cfg_sound[id..""]
    if soundCfg ~= nil then
        SoundManger:PlayUISound(soundCfg.SoundName, ESoundLayer.Background);
    end
end

function LogicTools.PlayAudio(id)
    local soundCfg = cfg_sound[id..""]
    if soundCfg ~= nil then
        SoundManger:PlayUISound(soundCfg.SoundName, ESoundLayer.LayerReplace);
    end
end

--获取UI界面打开音效
function LogicTools.GetUIPageSoundId(uiName)
    if LogicTools.UIMap == nil then
        LogicTools.UIMap = {}
        for k,v in pairs(cfg_ui) do
            local name = string.lower(v.Windows);
            LogicTools.UIMap[name] = v
        end
    end
    uiName = string.lower(uiName);
    uiName = string.gsub(uiName, "ui_","")

    local uiCfg = LogicTools.UIMap[uiName]
    if uiCfg then
        return uiCfg.SoundId
    end
    return 0
end

