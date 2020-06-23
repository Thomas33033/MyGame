using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 移动设备输入框的自适应组件
/// </summary>
public class ChatViewAdaptMobileKeyBoard : MonoBehaviour
{
    public InputField _inputField;
    public RectTransform adaptPanelRt;
    public Button sendBtn;

    float keyboardHeight_local = 0;
    float keyboardHeight_last = 0;
    /// <summary>
    /// 自适应（弹出输入框后整体抬高）的面板的初始位置
    /// </summary>
    private Vector2 _adaptPanelOriginPos;
    Vector2 screenPos;

    public static ChatViewAdaptMobileKeyBoard Create(GameObject attachRoot, InputField inputField)
    {
        ChatViewAdaptMobileKeyBoard instance = null;
        instance = attachRoot.AddComponent<ChatViewAdaptMobileKeyBoard>();
        instance._inputField = inputField;

        return instance;
    }

    private void Start()
    {
        _inputField.onEndEdit.AddListener(OnEndEdit);
        _inputField.onValueChanged.AddListener(OnValueChanged);
        _adaptPanelOriginPos = adaptPanelRt.anchoredPosition;
        _inputField.keyboardType = TouchScreenKeyboardType.Default;
        _inputField.shouldHideMobileInput = true;
    }

    private void Update()
    {
        if (_inputField.isFocused)
        {


#if UNITY_EDITOR
            keyboardHeight_local = 0;
#elif UNITY_ANDROID
            keyboardHeight_local = GetKeyboardHeightAndroid() - 170;
#elif UNITY_IOS
            keyboardHeight_local =  GetKeyboardHeightIOS();
#endif
            if (keyboardHeight_last != keyboardHeight_local)
            {
                keyboardHeight_last = keyboardHeight_local;

                float keyboardHeight = keyboardHeight_local * Display.main.systemHeight / Screen.height;
                float k = gameObject.GetComponentInParent<CanvasScaler>().GetComponent<RectTransform>().sizeDelta.y;

                float keyboardHeightUi = keyboardHeight * k / Display.main.systemHeight;

                screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, _inputField.transform.position);
                screenPos.y -= _inputField.GetComponent<RectTransform>().rect.height / 2;
                Debug.LogError(keyboardHeightUi + "    " + screenPos.y);
                if (keyboardHeightUi > screenPos.y)
                {
                    keyboardHeightUi = keyboardHeightUi - screenPos.y;
                    adaptPanelRt.anchoredPosition = Vector3.up * keyboardHeightUi;
                }
            }
        }
        else
        {
            keyboardHeight_last = 0;
        }
    }

    private void OnValueChanged(string arg0) { }


    /// <summary>
    /// 结束编辑事件，TouchScreenKeyboard.isFocused为false的时候
    /// </summary>
    /// <param name="currentInputString"></param>
    private void OnEndEdit(string currentInputString)
    {
        StartCoroutine(DelayMoveBack());
    }


    IEnumerator DelayMoveBack()
    {
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        if (sendBtn.gameObject == EventSystem.current.currentSelectedGameObject)
        {
            sendBtn.onClick.Invoke();
        }
        adaptPanelRt.anchoredPosition = _adaptPanelOriginPos;
    }

    /// <summary>
    /// 获取安卓平台上键盘的高度
    /// </summary>
    /// <returns></returns>
    public int GetKeyboardHeightAndroid()
    {
        using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            var unityPlayer = unityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer");
            var view = unityPlayer.Call<AndroidJavaObject>("getView");
            var dialog = unityPlayer.Get<AndroidJavaObject>("b");

            if (view == null || dialog == null)
                return 0;

            var decorHeight = 0;

            if (true)//includeInput
            {
                var decorView = dialog.Call<AndroidJavaObject>("getWindow").Call<AndroidJavaObject>("getDecorView");

                if (decorView != null)
                    decorHeight = decorView.Call<int>("getHeight");
            }

            using (var rect = new AndroidJavaObject("android.graphics.Rect"))
            {
                view.Call("getWindowVisibleDisplayFrame", rect);
                return Display.main.systemHeight - rect.Call<int>("height") + decorHeight;
            }
        }
    }



    public int GetKeyboardHeightIOS()
    {
        return (int)TouchScreenKeyboard.area.height;
    }
}
