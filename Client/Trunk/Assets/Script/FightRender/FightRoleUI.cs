using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 角色头顶UI渲染
/// </summary>
public class FightRoleUI : MonoBehaviour
{
    public Image imgHp;
    public Image imgMp;

    public int hpMax;
    public int mpMax;

    private void Awake()
    {
        imgHp.fillAmount = 1f;
        imgMp.fillAmount = 0f;
    }

    public void SetData(int hpMax, int mpMax)
    {
        this.hpMax = hpMax;
        this.mpMax = mpMax;


        gameObject.SetActive(false);
    }

    public void ShowHp(int hp)
    {
        if (hp < hpMax)
        {
            gameObject.SetActive(true);
        }
        Debug.LogError("ShowHp:"+  hp + " " + hpMax);
        imgHp.fillAmount = 1f * hp / hpMax;
    }

    public void ShowMp(int mp)
    {
        imgMp.fillAmount = 1f * mp / mpMax;
    }
}