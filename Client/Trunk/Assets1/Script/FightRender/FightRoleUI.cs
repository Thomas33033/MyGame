using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 角色头顶UI渲染
/// </summary>
public class FightRoleUI : MonoBehaviour
{
    public Image imgHp;
    public Image imgMp;

    public Image imgHpBg;

    public int hpMax;
    public int mpMax;

    private void Awake()
    {
        imgHp.fillAmount = 1f;
        imgMp.fillAmount = 0f;
    }

    public void SetData(int hpMax, int mpMax, bool isSelf)
    {
        this.hpMax = hpMax;
        this.mpMax = mpMax;

        imgHp.fillOrigin = isSelf ? (int)Image.OriginHorizontal.Left : (int)Image.OriginHorizontal.Right;
        imgMp.fillOrigin = isSelf ? (int)Image.OriginHorizontal.Left : (int)Image.OriginHorizontal.Right;

        //var sprite = LoadTools.LoadAtlasSprite("Atlas_Fight", isSelf ? "X_Bg_xt07" : "X_Bg_xt05");
        //imgHpBg.sprite = sprite;

        gameObject.SetActive(false);
    }

    public void ShowHp(int hp)
    {
        if (hp < hpMax)
        {
            gameObject.SetActive(true);
        }

        imgHp.fillAmount = 1f * hp / hpMax;
    }

    public void ShowMp(int mp)
    {
        imgMp.fillAmount = 1f * mp / mpMax;
    }
}