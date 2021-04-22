using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using Fight;
using Newtonsoft.Json;

public class SkillEditor : EditorWindow
{
    private string _newId;

    private int _indexList;

    private static string path = "Assets/BundleRes/Config/";
    private static string skillEditorConfig = "Assets/Editor/Fight/SkillEditorConfig.asset";
    private static string SkillEditorSkin = "Assets/Editor/Fight/SkillEditor.guiskin";

    private string[] attTriggerNames;
    private int[] attTriggerIndex;
    private Dictionary<int, string> dicTrigger;

    private GUISkin _styleGB;

    [MenuItem("GameTools/Open Skill Editor")]
    public static void Create()
    {
        SkillEditor myWindow = (SkillEditor)EditorWindow.GetWindow(typeof(SkillEditor), false, "SkillEditor", true);//创建窗口
        myWindow.Show();
    }

    private int _curEffectIndex;
    public List<FightEffectInfo> listEffectInfos;
    public string[] arrEffectIds;

    private Type _typeTriggerType;

    private bool _isListOpen;

    private Vector2 _vectorListOpen;

    private void Awake()
    {
        //GUI.skin.font.fontSize = 32;

        minSize = new Vector2(800f, 600f);

        GetData();

        _skillEditorConfig = AssetDatabase.LoadAssetAtPath<SkillEditorConfig>(skillEditorConfig);
        _skillActionNames = new string[_skillEditorConfig.list.Count + 1];
        _skillActionNames[0] = "";
        for (int i = 0; i < _skillEditorConfig.list.Count; i++)
        {
            _skillActionNames[i + 1] = _skillEditorConfig.list[i].id + "." + _skillEditorConfig.list[i].name;
        }

        _typeTriggerType = typeof(TriggerType);
        dicTrigger = new Dictionary<int, string>();
        foreach (int v in Enum.GetValues(_typeTriggerType))
        {
            if (TriggerName.ContainsKey(v))
            {
                dicTrigger.Add(v, TriggerName[v]);
            }
            else
            {
                string strName = v + "." + Enum.GetName(_typeTriggerType, v);
                dicTrigger.Add(v, strName);
            }
        }

        attTriggerNames = new List<string>(dicTrigger.Values).ToArray();
        attTriggerIndex = new List<int>(dicTrigger.Keys).ToArray();

        _styleGB = AssetDatabase.LoadAssetAtPath<GUISkin>(SkillEditorSkin);
    }

    private void GetData()
    {
        string txt = "";
        if (File.Exists(path + "FightEffects.json"))
        {
            txt = File.ReadAllText(path + "FightEffects.json");
        }

        listEffectInfos = new List<FightEffectInfo>(SimpleJson.SimpleJson.DeserializeObject<FightEffectInfo[]>(txt));

        UpdateEffectIds();
    }

    private void UpdateEffectIds()
    {
        listEffectInfos.Sort(SortListEffectInfos);

        arrEffectIds = new string[listEffectInfos.Count];

        for (int i = 0; i < listEffectInfos.Count; i++)
        {
            arrEffectIds[i] = listEffectInfos[i].id+"";
        }
        if (_curEffectIndex >= listEffectInfos.Count)
            _curEffectIndex = 0;
    }

    private int SortListEffectInfos(FightEffectInfo x, FightEffectInfo y)
    {
        //if (string.IsNullOrEmpty(_newId) == false)
        //{
        //    return _newId.CompareTo(x.id).CompareTo(_newId.CompareTo(y.id));
        //}

        return x.id.CompareTo(y.id);
    }

    private void OnDestroy()
    {
        AssetDatabase.Refresh();
    }

    private int _toolbar;

    private void OnGUI()
    {
        if (Application.isPlaying)
            return;

        _toolbar = GUILayout.Toolbar(_toolbar, new string[] { "技能编辑", "动作编辑", "属性说明" });

        if (_toolbar == 0)
        {
            OnGUIEffect();
        }
        else if (_toolbar == 1)
        {
            OnGUIAction();
        }
        else
        {
            EditorGUILayout.TextArea(AttrDesc);
        }

        EditorGUILayout.Space();
    }

    #region Action

    private Vector2 _v2Actionlist;

    private void OnGUIAction()
    {
        EditorGUILayout.BeginVertical();

        GUILayout.Space(20f);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("排序", GUILayout.MaxWidth(120)))
        {
            _skillEditorConfig.Sort();
        }

        if (GUILayout.Button("保存", GUILayout.MaxWidth(120)))
        {
            EditorUtility.SetDirty(_skillEditorConfig);

            _skillActionNames = new string[_skillEditorConfig.list.Count + 1];
            _skillActionNames[0] = "";
            for (int i = 0; i < _skillEditorConfig.list.Count; i++)
            {
                _skillActionNames[i + 1] = _skillEditorConfig.list[i].id + "." + _skillEditorConfig.list[i].name;
            }
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(20f);

        _v2Actionlist = EditorGUILayout.BeginScrollView(_v2Actionlist);

        for (int i = 0; i < _skillEditorConfig.list.Count; i++)
        {
            ShowEffectInfo(_skillEditorConfig.list[i]);
            if (GUILayout.Button("删除动作", GUILayout.MaxWidth(120)))
            {
                _skillEditorConfig.list.RemoveAt(i);
                i--;
            }
            GUILayout.Space(20f);
        }

        if (GUILayout.Button("添加动作", GUILayout.MaxWidth(120)))
        {
            _skillEditorConfig.list.Add(new SkillEditorEffectInfo());
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
    }

    private void ShowEffectInfo(SkillEditorEffectInfo info)
    {
        EditorGUILayout.BeginVertical(_styleGB.scrollView);

        info.id = EditorGUILayout.IntField("ID：", info.id);

        info.name = EditorGUILayout.TextField("名称：", info.name);

        info.desc = EditorGUILayout.TextField("备注：", info.desc);

        GUILayout.Space(5f);

        EditorGUILayout.LabelField("字段(字符串)", GUILayout.MaxWidth(240));

        for (int i = 0; i < info.keyNames.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("-", GUILayout.MaxWidth(60)))
            {
                info.keyNames.RemoveAt(i);
                i--;
            }
            info.keyNames[i] = EditorGUILayout.TextField(info.keyNames[i]);
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("+", GUILayout.MaxWidth(60)))
        {
            info.keyNames.Add("");
        }

        GUILayout.Space(5f);

        EditorGUILayout.LabelField("字段(数值)", GUILayout.MaxWidth(240));
        for (int i = 0; i < info.valueNames.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("-", GUILayout.MaxWidth(60)))
            {
                info.valueNames.RemoveAt(i);
                i--;
            }
            info.valueNames[i] = EditorGUILayout.TextField(info.valueNames[i]);
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("+", GUILayout.MaxWidth(60)))
        {
            info.valueNames.Add("");
        }

        EditorGUILayout.EndVertical();
    }

    #endregion Action

    private void OnGUIEffect()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("新建", GUILayout.MaxWidth(60));

        _newId = EditorGUILayout.TextField(_newId, GUILayout.MaxWidth(120));

        if (GUILayout.Button("新建", GUILayout.MaxWidth(120)))
        {
            _newId = _newId.Trim();

            if (string.IsNullOrEmpty(_newId))
            {
                Debug.LogError("ID不能为空！");
                return;
            }

            if (!string.IsNullOrEmpty(_newId))
            {
                for (int i = 0; i < listEffectInfos.Count; i++)
                {
                    if (listEffectInfos[i].id.ToString() == _newId)
                    {
                        Debug.LogError("技能ID重复");
                        return;
                    }
                }

                FightEffectInfo fightEffectInfo = new FightEffectInfo();
                fightEffectInfo.id = int.Parse(_newId);
                fightEffectInfo.actionInfos = new FightEffectActionInfo[0];
                listEffectInfos.Add(fightEffectInfo);
                UpdateEffectIds();
                _newId = "";
                _curEffectIndex = listEffectInfos.IndexOf(fightEffectInfo);
            }
        }

        if (GUILayout.Button("显示", GUILayout.MaxWidth(120)))
        {
            _newId = _newId.Trim();

            if (!string.IsNullOrEmpty(_newId))
            {
                for (int i = 0; i < listEffectInfos.Count; i++)
                {
                    if (listEffectInfos[i].id.ToString() == _newId)
                    {
                        _curEffectIndex = i;
                        return;
                    }
                }
                _curEffectIndex = -1;
            }
            else
            {
                _curEffectIndex = 0;
            }
            UpdateEffectIds();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("保存", GUILayout.MaxWidth(120)))
        {
            FightEffectInfo select = null;
            if (listEffectInfos.Count > 0)
                select = listEffectInfos[_curEffectIndex];

            listEffectInfos.Sort((FightEffectInfo a, FightEffectInfo b) => a.id.CompareTo(b.id));

            if (select != null)
                _curEffectIndex = listEffectInfos.IndexOf(select);

            string txt = Newtonsoft.Json.JsonConvert.SerializeObject(listEffectInfos, Formatting.Indented);
            File.WriteAllText(path + "FightEffects.json", txt);
            Debug.Log("已保存 " + path + "FightEffects.json");
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("修复", GUILayout.MaxWidth(120)))
        {
            Fix();
        }

        if (_isListOpen == false)
        {
            if (GUILayout.Button("展开", GUILayout.MaxWidth(240)))
            {
                _isListOpen = true;
            }
        }
        else
        {
            if (GUILayout.Button("收起", GUILayout.MaxWidth(240)))
            {
                _isListOpen = false;
            }
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (listEffectInfos.Count == 0 || _curEffectIndex == -1)
        {
            return;
        }

        if (_isListOpen)
        {
            EditorGUILayout.Space();

            _vectorListOpen = EditorGUILayout.BeginScrollView(_vectorListOpen, GUILayout.MaxHeight(360), GUILayout.MaxWidth(180));

            for (int i = 0; i < arrEffectIds.Length; i++)
            {
                if (GUILayout.Button(arrEffectIds[i], GUILayout.MaxWidth(360)))
                {
                    _curEffectIndex = i;
                    _isListOpen = false;
                }
            }

            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.Space();

        int lastIndex = _curEffectIndex;
        _curEffectIndex = EditorGUILayout.Popup(_curEffectIndex, arrEffectIds, GUILayout.MaxWidth(120));
        if (lastIndex != _curEffectIndex)
        {
            GetData();
        }

        EditorGUILayout.Space();

        ShowEffect(listEffectInfos[_curEffectIndex]);

        EditorGUILayout.EndVertical();
    }

    private Vector2 scrollActionPosition;

    private void ShowEffect(FightEffectInfo effectInfo)
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("ID:", GUILayout.MaxWidth(60));
        effectInfo.id = int.Parse(EditorGUILayout.TextField(effectInfo.id.ToString(), GUILayout.MaxWidth(180)));

        if (GUILayout.Button("删除", GUILayout.MaxWidth(120)))
        {
            listEffectInfos.Remove(effectInfo);
            UpdateEffectIds();
            return;
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("触发时机:", GUILayout.MaxWidth(60));
        string strName = dicTrigger[effectInfo.trigger];
        int triggerIndex = Array.IndexOf<string>(attTriggerNames, strName);
        triggerIndex = EditorGUILayout.Popup(triggerIndex, attTriggerNames, GUILayout.MaxWidth(180));

        effectInfo.trigger = attTriggerIndex[triggerIndex];

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("全局响应:", GUILayout.MaxWidth(60));
        effectInfo.space = EditorGUILayout.Toggle(effectInfo.space == 0, GUILayout.MaxWidth(180)) ? 0 : 1;

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        scrollActionPosition = EditorGUILayout.BeginScrollView(scrollActionPosition, GUILayout.MaxHeight(540));

        for (int i = 0; i < effectInfo.actionInfos.Length; i++)
        {
            effectInfo.actionInfos[i].step = i;
            ShowAction(effectInfo, effectInfo.actionInfos[i]);
            EditorGUILayout.Space();
        }

        if (GUILayout.Button("添加动作", GUILayout.MaxWidth(120)))
        {
            List<FightEffectActionInfo> temp = new List<FightEffectActionInfo>(effectInfo.actionInfos);
            temp.Add(new FightEffectActionInfo());
            effectInfo.actionInfos = temp.ToArray();
        }

        EditorGUILayout.EndScrollView();
    }

    private SkillEditorConfig _skillEditorConfig;
    private string[] _skillActionNames;

    private SkillEditorEffectInfo GetActionConfig(int id)
    {
        for (int i = 0; i < _skillEditorConfig.list.Count; i++)
        {
            if (_skillEditorConfig.list[i].id == id)
            {
                return _skillEditorConfig.list[i];
            }
        }
        return null;
    }

    private void ShowAction(FightEffectInfo effectInfo, FightEffectActionInfo actionInfo)
    {
        GUILayout.Box("步骤:" + actionInfo.step, GUILayout.MaxWidth(60));

        SkillEditorEffectInfo editorEffectInfo = GetActionConfig(actionInfo.id);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("动作类型:", GUILayout.MaxWidth(180));

        int lastIndex = 0;
        if (actionInfo.id != 0)
        {
            lastIndex = Array.IndexOf<string>(_skillActionNames, editorEffectInfo.id + "." + editorEffectInfo.name);
        }

        int index = EditorGUILayout.Popup(lastIndex, _skillActionNames, GUILayout.MaxWidth(300));

        if (lastIndex != index)
        {
            editorEffectInfo = _skillEditorConfig.list[index - 1];
            actionInfo.id = editorEffectInfo.id;
            actionInfo.values = new int[editorEffectInfo.valueNames.Count];
            actionInfo.valuesType = new int[editorEffectInfo.valueNames.Count];
            actionInfo.keys = new string[editorEffectInfo.keyNames.Count];
        }

        if (editorEffectInfo != null)
        {
            EditorGUILayout.LabelField(editorEffectInfo.desc);
        }

        EditorGUILayout.EndHorizontal();

        if (editorEffectInfo != null)
        {
            if (actionInfo.values.Length < editorEffectInfo.valueNames.Count)
            {
                List<int> listTemp = new List<int>(actionInfo.values);
                List<int> listTemp2 = new List<int>(actionInfo.valuesType);
                int numTemp = editorEffectInfo.valueNames.Count - listTemp.Count;
                for (int i = 0; i < numTemp; i++)
                {
                    listTemp.Add(0);
                    listTemp2.Add(0);
                }
                actionInfo.values = listTemp.ToArray();
                actionInfo.valuesType = listTemp2.ToArray();
            }

            for (int i = 0; i < editorEffectInfo.valueNames.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                actionInfo.valuesType[i] = EditorGUILayout.Toggle(actionInfo.valuesType[i] == 1, GUILayout.MaxWidth(20)) ? 1 : 0;

                string[] arr = editorEffectInfo.valueNames[i].Split('#');
                EditorGUILayout.LabelField(arr[0], GUILayout.MaxWidth(180));
                actionInfo.values[i] = EditorGUILayout.IntField(actionInfo.values[i], GUILayout.MaxWidth(180));
                if (arr.Length > 1)
                    EditorGUILayout.LabelField(arr[1]);

                EditorGUILayout.EndHorizontal();
            }

            if (actionInfo.keys.Length < editorEffectInfo.keyNames.Count)
            {
                List<string> listTemp = new List<string>(actionInfo.keys);
                int numTemp = editorEffectInfo.keyNames.Count - listTemp.Count;
                for (int i = 0; i < numTemp; i++)
                {
                    listTemp.Add("");
                }
                actionInfo.keys = listTemp.ToArray();
            }

            for (int i = 0; i < editorEffectInfo.keyNames.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                string[] arr = editorEffectInfo.keyNames[i].Split('#');
                EditorGUILayout.LabelField(arr[0], GUILayout.MaxWidth(180));
                actionInfo.keys[i] = EditorGUILayout.TextField(actionInfo.keys[i], GUILayout.MaxWidth(180));
                if (arr.Length > 1)
                    EditorGUILayout.LabelField(arr[1], GUILayout.MaxWidth(180));
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical();

        if (GUILayout.Button("上移", GUILayout.MaxWidth(60)))
        {
            if (actionInfo.step > 0)
            {
                FightEffectActionInfo temp = effectInfo.actionInfos[actionInfo.step - 1];
                effectInfo.actionInfos[actionInfo.step - 1] = actionInfo;
                effectInfo.actionInfos[actionInfo.step] = temp;
            }
        }

        if (GUILayout.Button("下移", GUILayout.MaxWidth(60)))
        {
            if (actionInfo.step < effectInfo.actionInfos.Length - 1)
            {
                FightEffectActionInfo temp = effectInfo.actionInfos[actionInfo.step + 1];
                effectInfo.actionInfos[actionInfo.step + 1] = actionInfo;
                effectInfo.actionInfos[actionInfo.step] = temp;
            }
        }

        if (GUILayout.Button("删除", GUILayout.MaxWidth(60)))
        {
            List<FightEffectActionInfo> temp = new List<FightEffectActionInfo>(effectInfo.actionInfos);
            temp.RemoveAt(actionInfo.step);

            effectInfo.actionInfos = temp.ToArray();
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    private void Fix()
    {
        for (int i = 0; i < listEffectInfos.Count; i++)
        {
            for (int j = 0; j < listEffectInfos[i].actionInfos.Length; j++)
            {
                Fix(listEffectInfos[i].actionInfos[j]);
            }
        }
    }

    private void Fix(FightEffectActionInfo actionInfo)
    {
        SkillEditorEffectInfo editorEffectInfo = GetActionConfig(actionInfo.id);

        if (editorEffectInfo != null)
        {
            if (actionInfo.values.Length < editorEffectInfo.valueNames.Count)
            {
                List<int> listTemp = new List<int>(actionInfo.values);
                List<int> listTemp2 = new List<int>(actionInfo.valuesType);
                int numTemp = editorEffectInfo.valueNames.Count - listTemp.Count;
                for (int i = 0; i < numTemp; i++)
                {
                    listTemp.Add(0);
                    listTemp2.Add(0);
                }
                actionInfo.values = listTemp.ToArray();
                actionInfo.valuesType = listTemp2.ToArray();
            }

            for (int i = 0; i < editorEffectInfo.valueNames.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                actionInfo.valuesType[i] = EditorGUILayout.Toggle(actionInfo.valuesType[i] == 1, GUILayout.MaxWidth(20)) ? 1 : 0;

                string[] arr = editorEffectInfo.valueNames[i].Split('#');
                EditorGUILayout.LabelField(arr[0], GUILayout.MaxWidth(180));
                actionInfo.values[i] = EditorGUILayout.IntField(actionInfo.values[i], GUILayout.MaxWidth(180));
                if (arr.Length > 1)
                    EditorGUILayout.LabelField(arr[1]);

                EditorGUILayout.EndHorizontal();
            }

            if (actionInfo.keys.Length < editorEffectInfo.keyNames.Count)
            {
                List<string> listTemp = new List<string>(actionInfo.keys);
                int numTemp = editorEffectInfo.keyNames.Count - listTemp.Count;
                for (int i = 0; i < numTemp; i++)
                {
                    listTemp.Add("");
                }
                actionInfo.keys = listTemp.ToArray();
            }

            for (int i = 0; i < editorEffectInfo.keyNames.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                string[] arr = editorEffectInfo.keyNames[i].Split('#');
                EditorGUILayout.LabelField(arr[0], GUILayout.MaxWidth(180));
                actionInfo.keys[i] = EditorGUILayout.TextField(actionInfo.keys[i], GUILayout.MaxWidth(180)).Trim();
                if (arr.Length > 1)
                    EditorGUILayout.LabelField(arr[1], GUILayout.MaxWidth(180));
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    public Dictionary<int, string> TriggerName = new Dictionary<int, string>() {
        {(int)TriggerType.None,"无"}
        ,{(int)TriggerType.AttackAfter,(int)TriggerType.AttackAfter + ".角色普攻后"}
        ,{(int)TriggerType.AttackBefore,(int)TriggerType.AttackBefore + ".角色普攻前"}
        ,{(int)TriggerType.AttackDataAttackerHitBefore,(int)TriggerType.AttackDataAttackerHitBefore + ".Damage命中前"}
        ,{(int)TriggerType.AttackDataExecuted,(int)TriggerType.AttackDataAttackerHitBefore + ".Damage(Type=0)到达目标时"}
        ,{(int)TriggerType.Time,(int)TriggerType.Time + ".时间"}
        ,{(int)TriggerType.Skill,(int)TriggerType.Skill + ".角色释放技能后"}
        ,{(int)TriggerType.AttackDataTargetHurtBefore,(int)TriggerType.AttackDataTargetHurtBefore + ".角色受到Damage伤害前"}
        ,{(int)TriggerType.AttackDataTargetDamageBefore,(int)TriggerType.AttackDataTargetDamageBefore + ".角色受到Damage伤害计算前"}
        ,{(int)TriggerType.AttackDataTargetHurtAfter,(int)TriggerType.AttackDataTargetHurtAfter + ".角色受到Damage伤害后"}
        ,{(int)TriggerType.AttackDataAttackerDamageBefore,(int)TriggerType.AttackDataAttackerDamageBefore + ".角色Damage伤害计算前"}
        ,{(int)TriggerType.Buff,(int)TriggerType.Buff + ".Buff执行后"}
        ,{(int)TriggerType.BuffStack,(int)TriggerType.BuffStack + ".Buff叠满后"}
        ,{(int)TriggerType.AttackDataAttackerHit,(int)TriggerType.AttackDataAttackerHit + ".角色Damage伤害命中后"}
        ,{(int)TriggerType.AttackDataTargetHit,(int)TriggerType.AttackDataTargetHit + ".角色受到Damage伤害命中后"}
        ,{(int)TriggerType.Kill,(int)TriggerType.Kill + ".角色击杀后"}
        ,{(int)TriggerType.PrepareFight,(int)TriggerType.PrepareFight + ".准备战斗"}
        ,{(int)TriggerType.Init,(int)TriggerType.Init + ".初始化"}
        ,{(int)TriggerType.AttackDataTargetCureBefore,(int)TriggerType.AttackDataTargetCureBefore + ".角色受到Damage治疗前"}
        ,{(int)TriggerType.AttackDataAttackerCureHit,(int)TriggerType.AttackDataAttackerCureHit + ".角色Damage治疗命中后"}
        ,{(int)TriggerType.AttackDataTargetCureHit,(int)TriggerType.AttackDataTargetCureHit + ".角色受到Damage治疗命中后"}
        ,{(int)TriggerType.AttackDataAttackerCureBefore,(int)TriggerType.AttackDataAttackerCureBefore + ".角色Damage治疗计算前"}
        ,{(int)TriggerType.AuraAdd,(int)TriggerType.AuraAdd + ".光环添加角色时"}
        ,{(int)TriggerType.AuraRemove,(int)TriggerType.AuraRemove + ".光环移除角色时"}
        ,{(int)TriggerType.AuraEnter,(int)TriggerType.AuraEnter + ".角色受到光环时"}
        ,{(int)TriggerType.AuraExit,(int)TriggerType.AuraEnter + ".角色离开光环时"}
        ,{(int)TriggerType.AuraHas,(int)TriggerType.AuraHas + ".光环有角色时"}
        ,{(int)TriggerType.AuraNone,(int)TriggerType.AuraNone + ".光环没有角色时"}
        ,{(int)TriggerType.AuraCreate,(int)TriggerType.AuraCreate + ".角色开启光环时"}
        ,{(int)TriggerType.AuraEnd,(int)TriggerType.AuraEnd + ".角色关闭光环时"}
        ,{(int)TriggerType.Die,(int)TriggerType.Die + ".角色死亡时"}
        ,{(int)TriggerType.Dying,(int)TriggerType.Dying + ".角色将要死亡时"}
        ,{(int)TriggerType.ShieldCreated,(int)TriggerType.ShieldCreated + ".角色创建护盾时"}
        ,{(int)TriggerType.TargetChange,(int)TriggerType.TargetChange + ".角色切换目标时"}
        ,{(int)TriggerType.MpMax,(int)TriggerType.MpMax + ".怒气满时"}
    };

    public string AttrDesc = @"
mp 魔法

attack 物理攻击

attackSpeed 攻速

crit    暴击

defense 物理防御

magicDefense    魔抗

dodge   闪避

hit 命中

hp  血量

range   射程

moveSpeed   移动速度

damageReduction 伤害减免

damageBouns 伤害增加

defensePenetration  护甲穿透

magicDefensePenetration 魔抗穿透

defenseDestroy  忽视护甲

magicDefenseDestroy 忽视魔抗

unselected  不可选中

undead  不死

immuneDebuff    免疫负面效果

silent  沉默

hpSucking   吸血
";
}