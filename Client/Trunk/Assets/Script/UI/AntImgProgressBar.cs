using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class AntImgProgressBar : MonoBehaviour
{
    private Image _img;

    public float speed;

    public float value
    {
        set
        {
            _img.fillAmount = value;
        }
        get
        {
            return _img.fillAmount;
        }
    }

    private Tweener _tweener;

    private void Awake()
    {
        _img = GetComponent<Image>();
    }

    public void TweenValue(float v,float time)
    {
        if (_tweener != null)
            _tweener.Kill();

        _tweener = _img.DOFillAmount(v, time);
    }

    public void Test(float v)
    {
        TweenValue(v, 2);
    }

    public void SetSpeed(float v)
    {
        speed = v;
    }

    private void Update()
    {
        value += speed * Time.deltaTime;
    }

}
