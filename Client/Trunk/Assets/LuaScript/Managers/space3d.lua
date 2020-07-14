---场景管理
--场景加载、卸载
--场景叠加，根据创建顺序依次存入队列

Space3D = {}

local arrTempSprite = {}
LoadTools.SetAssetBundleKeepTime(0,1)

local arrObj = {}


Space3D.curName = ""
Space3D.lastName = ""

local function DoLoadSpace(uiTable)
	local loadSceneMode = uiTable.loadSceneMode  or 0

	if StaticData.ELoadSceneMode.Single == loadSceneMode then
		UI.RemoveAll()
		Space3D.RemoveAll()
	elseif StaticData.ELoadSceneMode.Additive == loadSceneMode then

	end


	Space3D.lastName = Space3D.curName
	Space3D.curName = uiTable.sceneName or uiTable.name
	-- Space3D.curSceneName = "SC_"..uiTable.name

	LoadGameScene("SC_"..uiTable.name,loadSceneMode,function ()
		local obj =  UnityEngine.GameObject.Find(uiTable.name)

		if obj ~= nil then
			uiTable.gameObject = obj
			uiTable.transform = obj.transform
			UI.FindUICompent(uiTable,uiTable.transform)
			uiTable.luaUI = obj:AddComponent(LuaSpaceComponent)
			uiTable.luaUI:SetData(uiTable)
		end

		table.insert(arrObj,uiTable)

		uiTable:Awake()

		Space3D.SetSpaceUIEnabled(uiTable)

		if StaticData.ELoadSceneMode.Additive == loadSceneMode then
			local lastTable = Space3D.GetSpaceTable(Space3D.lastName)
			if lastTable ~= nil then
				UIHelper.SetActive(lastTable.transform,false)
				if lastTable.OnDisable ~= nil then
					lastTable:OnDisable()
				end
			end
		end
	end,0)
end

function Space3D.LoadSpace(uiTable)
	if uiTable.needLoading == true then
		local uiBlackLoading = UI.OpenOrGetUI("UIBlackLoading")
		LuaTools.DelayCall(0.1,function ()
			DoLoadSpace(uiTable)
		end)
		uiTable.uiLoading = uiBlackLoading
	else
		DoLoadSpace(uiTable)
	end
end

local function DoDestroySpace3D(uiTable)
	local sname = uiTable.sceneName or uiTable.name
	UnloadGameScene("SC_"..sname)
	uiTable:OnClose()
	if uiTable.gameObject ~= nil then
		UnityEngine.GameObject.Destroy(uiTable.gameObject)
	end
	uiTable:Clear()
end

function Space3D.RemoveCurSpace()
	local curTable = Space3D.GetSpaceTable(Space3D.curName)
	if curTable ~= nil then
		UI.RemoveAll()
		Space3D.RemoveSpace(curTable)
	end
end

function Space3D.GetSpaceTable(uiTableName)
	if uiTableName == "" or uiTableName == nil then
		return nil
	end

	for i=#arrObj,1,-1 do
		if arrObj[i].name == uiTableName then
			return arrObj[i]
		end
	end

	return nil
end


function Space3D.InsertSpaceToEnd(uiTable)
	if uiTable == nil then
		return nil
	end

	for i=#arrObj,1,-1 do
		if arrObj[i] == uiTable then
			table.remove(arrObj,i)
			break;
		end
	end
	table.insert(arrObj,uiTable)
end



function Space3D.RemoveSpace(uiTable)
	if uiTable == nil then
		return;
	end

	for i=#arrObj,1,-1 do
		if arrObj[i] == uiTable then
			table.remove(arrObj,i)
			break
		end
	end
	DoDestroySpace3D(uiTable)
	if #arrObj > 0 then
		local portTable = arrObj[#arrObj]
		Space3D.curName = portTable.name
		UIHelper.SetActive(portTable.transform,true)
		portTable:OnEnable()
	end
end


function Space3D.RemoveAll()
	for i=#arrObj,1,-1 do
		local temp = arrObj[i]
		if temp.keep ~= true then
			table.remove(arrObj,i)
			DoDestroySpace3D(temp)
		end
	end

	if #arrObj > 0 then
		Space3D.curName = arrObj[#arrObj].name
	else
		for i,v in ipairs(arrTempSprite) do
			v:Unlink()
		end
		arrTempSprite = {}

		collectgarbage("collect")
		LoadTools.ClearAssetBundle()
		LoadTools.ClearSceneCache()
		UnityEngine.Resources.UnloadUnusedAssets()
	end
end

function Space3D.StartSpace(v)
	v:Start()
	--Event.Call(EventType.SpaceChange,v.name)
	--GameData.guide:Run()
	if v.uiLoading ~= nil then
		v.uiLoading:CanClose(function ()
			if v.EnterScene ~= nil then
				v:EnterScene()
			end
		end)
		v.uiLoading = nil
	end
end

function Space3D.UIHide(v)
	for i,table in ipairs(arrObj) do
		local canvasGroup = table.gameObject:GetComponent("CanvasGroup")
		if canvasGroup == nil then
			canvasGroup = table.gameObject:AddComponent("UnityEngine.CanvasGroup")
		end

		if v then
			canvasGroup.alpha = 1
		else 
			canvasGroup.alpha = 0
		end
	end
end

local enabledSpaceUI = true

function Space3D.SetEnable(value)
	enabledSpaceUI = value

	for i,table in ipairs(arrObj) do
		Space3D.SetSpaceUIEnabled(table)
	end
end

function Space3D.SetSpaceUIEnabled(table)
	if table.gameObject == nil then
		return
	end

	local canvasGroup = table.gameObject:GetComponent("CanvasGroup")
	if canvasGroup == nil then
		canvasGroup = table.gameObject:AddComponent("UnityEngine.CanvasGroup")
	end
	canvasGroup.blocksRaycasts = enabledSpaceUI
end

function LoadAtlasSprite(path,name)
	local sprite = LoadTools.LoadAtlasSprite(path,name)
	table.insert(arrTempSprite,sprite)
	return sprite
end

function LoadGameScene(name,type,cb,dependencies)
	print(">> LoadGameScene",name)
	UI.SetLoading(true,1)
	UI.RemoveAll()
	LoadTools.LoadAssetBundleScene(name,type,function()
		UI.SetLoading(false,1)
		cb()
	end,1)
end

function UnloadGameScene(name)
	if name ~= nil and name ~= "" then
		LoadTools.UnloadSceneAsync(name)
	end
end

function DoRelogin(cb)
	if BattleMap.curMap ~= nil then
		BattleMap.curMap.keep = false
	end
	Net.SetCount(0,0)
	Space3D.RemoveAll()
	UI.Clearn()
	UI.SetEnable(true)

	LoadGameScene("SC_Empty",0,function ()
		LoadTools.LoadScene("SC_LoadResources",0,cb)
	end,0)
end


SDKFly.SdkTool.SetLogoutCallBack(function ( ... )
		DoRelogin(nil)
	end)