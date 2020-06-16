using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ParticleRippleCoins : MonoBehaviour
{
    enum CoinType
    {
        CoinTypeLine,
        CoinTypeRipple,
    }

    List<Coin> mCoins = new List<Coin>();
    float mRippleDuration = 0.2f;
    public float mMoveSpeed = 100;
    public float mPauseDuration = 0.3f;
    public float mJumpHeight = 50.0f;
    public float mJumpDuration = 0.7f;
    public float mEachDelay = 0.3f;

    public GameObject initPosObj;
    public GameObject targetPosObj;

    private Vector3 mInitPosition;
    private Vector3 mTargetPosition;
    public int mCoinsCount;
    private float mLayer = 0;

    void Start()
    {
        GameObject.Find("root").transform.position = this.transform.position;
        mInitPosition = initPosObj.transform.position;
        mTargetPosition = targetPosObj.transform.position;
        create();
    }

    public void create()
    {
        
    }

    public void lineInit()
    {
        float gap = 10f;
        mCoinsCount = 1;
        float left_distance = -gap * (float)mCoinsCount / 2f;

        for (int i = 0; i < mCoinsCount; i++)
        {
            mCoins.Add(
                newCoin(mInitPosition, mInitPosition + new Vector3(left_distance + (i + 1) * gap, 0, 0),
                mTargetPosition, i * mEachDelay, CoinType.CoinTypeLine));
        }
    }

    public void rippleInit()
    {
        float init_angle = Random.Range(0, 90);
        float each_angle = 360f / (float)mCoinsCount;

        float radiusMax = 40;
        float radiusMin = 5;

        for (int i = 0; i < mCoinsCount; i++)
        {
            float angle_current = init_angle + each_angle * i * Mathf.Deg2Rad;
            mCoins.Add(
                newCoin(mInitPosition, mInitPosition + Random.Range(radiusMin, radiusMax) * new Vector3(Mathf.Cos(angle_current), Mathf.Sin(angle_current), 0),
                mTargetPosition, i * mEachDelay, CoinType.CoinTypeRipple));
        }
    }

    Coin newCoin(Vector3 init_pos, Vector3 ripple_target_pos, Vector3 move_target_pos, float spring_delay, CoinType coinType)
    {
        ActionSequence actionSequence = new ActionSequence();
        Coin coin = null;
        if (mCoinsCount == 1)
        {
            float mMoveDuration = Vector3.Distance(mInitPosition, mTargetPosition) / mMoveSpeed;
            actionSequence.AddAction(new ActionMove(move_target_pos, mMoveDuration));
            coin = new Coin(getPrefabName(), init_pos);
            coin.StartAction(actionSequence);
        }
        else if (coinType == CoinType.CoinTypeLine)
        {
            actionSequence.AddAction(new ActionVisible(false));
            actionSequence.AddAction(new ActionDelay(spring_delay));
            actionSequence.AddAction(new ActionVisible(true));
            actionSequence.AddAction(new ActionMove(ripple_target_pos, mRippleDuration));
            actionSequence.AddAction(new ActionJump(mJumpDuration, mJumpHeight, 1));
            actionSequence.AddAction(new ActionJump(mJumpDuration * 0.8f, mJumpHeight * 0.8f, 1));
            actionSequence.AddAction(new ActionJump(mJumpDuration, mJumpHeight / 4, 1));
            actionSequence.AddAction(new ActionDelay(mPauseDuration));
            float mMoveDuration = Vector3.Distance(mInitPosition, mTargetPosition) / mMoveSpeed;
            actionSequence.AddAction(new ActionMove(move_target_pos, mMoveDuration));

            coin = new Coin(getPrefabName(), ripple_target_pos);
            coin.StartAction(actionSequence);
        }
        else
        {
            actionSequence.AddAction(new ActionVisible(false));
            actionSequence.AddAction(new ActionVisible(true));
            actionSequence.AddAction(new ActionMove(ripple_target_pos, mRippleDuration));
            actionSequence.AddAction(new ActionDelay(mPauseDuration));

            float flyDuration = Vector3.Distance(move_target_pos, ripple_target_pos) / mMoveSpeed;
            actionSequence.AddAction(new ActionMove(move_target_pos, flyDuration));

            coin = new Coin(getPrefabName(), init_pos);
            coin.StartAction(actionSequence);
        }
        return coin;
    }

    string getPrefabName()
    {
        return "";
    }

    public void OnDestroy()
    {
        for (int i = 0; i < mCoins.Count; i++)
        {
            mCoins[i].DestroyParticle();
        }
        mCoins.Clear();
    }

    public void Update()
    {
        bool isDone = true;
        for (int i = 0; i < mCoins.Count; i++)
        {
            mCoins[i].update(Time.deltaTime);
            if (!mCoins[i].isDone())
            {
                isDone = false;
            }
        }

        if (isDone)
        {
           OnDestroy();
        }
    }
}

class Coin
{
    ActionSequence mActionSequence = new ActionSequence();
    GameObject mParticle;
    private ModelPoolObj mPpoolObj;

    public Coin(string prefabName, Vector3 initPos)
    {
        var pool = ObjectPoolManager.Instance.CreatePool<ModelPoolObj>("Assets/ThirdParty/UI/EXyinbi.prefab");
        mPpoolObj = pool.GetObject();
        mParticle = mPpoolObj.itemObj;

        mParticle.SetActive(true);
        Transform parent = GameObject.Find("root").transform;
        mParticle.transform.SetParent(parent);
        Debug.LogError(initPos + " " + parent.transform.position);
        mParticle.transform.position = initPos;
    }
    
    ~Coin()
    {
        DestroyParticle();
    }

    public void DestroyParticle()
    {
        if (mParticle != null)
        {
            mPpoolObj.ReturnToPool();
            mParticle = null;
            mParticle = null;
        }
    }

    public void update(float dt)
    {
        mActionSequence.Update(dt);
    }

    public void StartAction(ActionSequence actionSequence)
    {
        mActionSequence = actionSequence;
        mActionSequence.Start(mParticle);
    }

    public bool isDone()
    {
        return mActionSequence.IsDone();
    }
}
