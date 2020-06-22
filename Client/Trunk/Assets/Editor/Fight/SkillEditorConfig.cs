using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SkillEditorConfig : ScriptableObject
{
    public List<SkillEditorEffectInfo> list;

    [MenuItem("GameTools/Create SkillConfig")]
    public static void Create()
    {
        SkillEditorConfig asset = AssetDatabase.LoadAssetAtPath<SkillEditorConfig>("Assets/Editor/SkillEditorConfig.asset");
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<SkillEditorConfig>();
            UnityEditor.AssetDatabase.CreateAsset(asset, "Assets/Editor/SkillEditorConfig.asset");
        }
    }

    [ContextMenu("排序")]
    public void Sort()
    {
        list.Sort((SkillEditorEffectInfo x, SkillEditorEffectInfo y) =>
        {
            return x.id.CompareTo(y.id);
        });
    }
}

[System.Serializable]
public class SkillEditorEffectInfo
{
    public int id;
    public string name;
    public string desc;
    public List<string> valueNames;
    public List<string> keyNames;

    public SkillEditorEffectInfo()
    {
        valueNames = new List<string>();
        keyNames = new List<string>();
    }
}