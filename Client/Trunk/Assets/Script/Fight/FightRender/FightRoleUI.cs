using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

      //  gameObject.SetActive(false);
    }

    public void ShowHp(int hp)
    {
        
        if (hp < hpMax)
        {
           gameObject.SetActive(true);
        }

        imgHp.DOKill(true);
        imgHp.DOFillAmount(1f * hp / hpMax, 1);
    }

    public void ShowMp(int mp)
    {
        imgMp.DOKill(true);
        imgMp.DOFillAmount(1f * mp / mpMax, 1);
        hpStartTimes = 0;
    }

    private float hpDuration = 1;
    private float hpStartTimes = 0;
    private float speed = 1; 
    private float startValue;
    private float endValue;
    private float rate = 0;
    void Update()
    {
        //if (rate <= 1)
        //{
        //    hpStartTimes += Time.deltaTime * speed;
        //    imgMp.fillAmount = endValue + (rate) * (hpStartTimes - endValue);
        //}
        //rate = 1 - hpStartTimes / hpDuration;
    }
}