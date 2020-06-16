using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 实体UI组件
/// 用于:血条、表情、对话等等
/// </summary>
public class UIComponent : ComponentBase
{
    private GameObject uiRoot;
    private Text text;
    private Scrollbar scrollbar;

    public override void OnInit(CharacterBase p_owner)
    {
        base.OnInit(p_owner);
    }


    public override void OnUpdate(float dt)
    {
        base.OnUpdate(dt);
    }

    public void CreateHP()
    {
        string path = string.Format("{0}/{1}.prefab", ResPathHelper.UI_WINDOW_PATH, "UIRoot3D");

        var pool = ObjectPoolManager.Instance.CreatePool<ModelPoolObj>(path);
        CObjectPool<ModelPoolObj> modelPool = pool;
        ModelPoolObj modelPoolObj = modelPool.GetObject();
        uiRoot = modelPoolObj.itemObj;
        uiRoot.transform.parent = Owner.Trans.Find("head");
        uiRoot.transform.localPosition = new Vector3(0, 1, 0);
        GameObject scrollBarObj = uiRoot.transform.Find("Hp").gameObject;
        scrollbar = scrollBarObj.gameObject.GetComponent<Scrollbar>();
        this.RefreshHP();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (this.uiRoot != null && this.uiRoot.transform != null)
        {
            this.uiRoot.transform.parent = null;
            GameObject.Destroy(this.uiRoot);
        }
    }


    public void SetHp(float rate)
    {
        if (scrollbar != null)
            scrollbar.size = rate;
    }

    public void IsShowHp(bool state)
    {
        if(scrollbar != null)
        {
            scrollbar.gameObject.SetActive(state);
        }
    }


    //刷新血条
    public void RefreshHP()
    {
        //DebugMgr.LogError(this.Owner.HP + " " + this.Owner.MaxHP + " " + this.Owner.HP / this.Owner.MaxHP);
        this.SetHp(this.Owner.HP / this.Owner.MaxHP);

        if (this.Owner.HP >= this.Owner.MaxHP)
        {
            this.IsShowHp(true);
        }
        else
        {
            this.IsShowHp(true);
        }
    }

}
