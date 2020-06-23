using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FightDamageLabel : MonoBehaviour
{
    public Text txt;

    private Vector3 _endOffset;
    private Ease _ease;

    private DamagePoolObj _poolObj;

    public void OnStart()
    {
        transform.DOMove(transform.position + _endOffset, 1f).SetEase(_ease).OnComplete(DestroySelf);
    }
    public void SetText(string v, Vector3 endOffset, Ease ease)
    {
        txt.text = v;
        this._endOffset = endOffset;
        this._ease = ease;
    }

    public void SetPoolObj(DamagePoolObj poolObj)
    {
        this._poolObj = poolObj;
    }
    public void SetSkillHurt(int v)
    {
        if (v >= 0)
        {
            txt.color = new Color(1f, 0.7f, 0f);
            txt.text = "-" + v;
        }
        else
        {
            txt.color = Color.green;
            txt.text = "+" + (-v);
        }
        transform.DOMove(transform.position + new Vector3(0.5f, 1.5f, 0f), 1f).SetEase(Ease.InSine).OnComplete(DestroySelf);
    }

    public void SetBuffHurt(int v)
    {
        txt.color = new Color(242f / 255f, 90f / 255f, 90f / 255f);
        txt.text = "-" + v;
        transform.DOMove(transform.position + new Vector3(0.5f, 1f, 0f), 1f).SetEase(Ease.InSine).OnComplete(DestroySelf);
    }

    public void SetBuffCure(int v)
    {
        txt.color = Color.green;
        txt.text = "+" + v;
        transform.DOMove(transform.position + new Vector3(0.5f, 1f, 0f), 1f).SetEase(Ease.InSine).OnComplete(DestroySelf);
    }

    public void SetHurt(int v)
    {
        if (v >= 0)
        {
            txt.color = Color.red;
            txt.text = "-" + v;
        }
        else
        {
            txt.color = Color.green;
            txt.text = "+" + Mathf.Abs(v);
        }
        transform.DOMove(transform.position + new Vector3(0f, 1f, 0f), 1f).SetEase(Ease.OutBack).OnComplete(DestroySelf);
    }

    public void SetHurtCrit(int v)
    {
        if (v >= 0)
        {
            txt.color = Color.red;
            txt.text = "-" + v;
        }
        else
        {
            txt.color = Color.green;
            txt.text = "+" + Mathf.Abs(v);
        }
        transform.DOMove(transform.position + new Vector3(1f, 1f, 0f), 1f).SetEase(Ease.OutBack).OnComplete(DestroySelf);
    }

    private void DestroySelf()
    {
        this._poolObj.ReturnToPool();
    }

    public void SizeNear()
    {
        transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
    }

    public void SizeFar()
    {
        //transform.localScale = new Vector3(1f, 1f, 2f);
    }
}