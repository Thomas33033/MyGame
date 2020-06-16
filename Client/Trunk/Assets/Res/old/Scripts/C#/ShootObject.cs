using UnityEngine;
using System.Collections;

public enum _ShootObjectType
{
    Projectile,   //抛射
    Missile,      //导弹
    Beam,         //电波
    Effect,       //特效
}
//子弹移动类
public class ShootObject : MonoBehaviour
{

    public _ShootObjectType type;
    public ModelPoolObj poolItem;
    public EffectPoolObj hitPoolItem;

    public CharacterBase target;
    private CharacterBase srcTower;
    private SkillClipData skillClipData;
    private Transform shootPoint;

    private bool hit = true;

    private GameObject thisObj;
    private Transform thisT;

    //for instant shootObject
    public LineRenderer lineRenderer;
    public float beamLength = Mathf.Infinity;   //电波长度
    public float duration = 0;                  //持续时间
    public bool continousDamage = false;        //连续伤害

    private TrailRenderer trail;              //追踪渲染
    private float trailDuration;              //追踪时间

    public AudioClip shootAudio;              //射击音效
    public AudioClip hitAudio;                //命中音效          

    public string shootEffect;            //射击特效
    public string hitEffect;              //命中特效  


    public int pType = 1;

    [SerializeField] private ParticleSystem pSystem;
    private float startLifeTime;
    private bool looping;

    //[SerializeField] private ParticleEmitter pEmitter;
    private float minE = 0;
    private float maxE = 0;

    private float particleDuration;

    public bool oneShot = false;

    public void SetupParticleEmitter()
    {
        pType = 2;
        //pEmitter=thisObj.GetComponent<ParticleEmitter>();
        //Debug.Log("get ParticleEmitter   "+pEmitter);
    }
    //public ParticleEmitter GetParticleEmitter(){
    //	return pEmitter;
    //}

    public void SetupParticleSystem()
    {
        pType = 1;
        pSystem = thisObj.GetComponent<ParticleSystem>();
    }

    public ParticleSystem GetParticleSystem()
    {
        return pSystem;
    }

    public RoutineComponent routineComponent;

    void Awake()
    {
        thisObj = gameObject;
        thisT = transform;

        trail = thisObj.GetComponent<TrailRenderer>();
        if (trail != null) trailDuration = trail.time;

        if (type == _ShootObjectType.Effect)
        {

            if (pType == 1)
            {
                pSystem = thisObj.GetComponent<ParticleSystem>();
                particleDuration = pSystem.main.duration + pSystem.main.startLifetimeMultiplier;
                looping = pSystem.main.loop;
            }

            if (pType == 2)
            {
                //pEmitter=thisObj.GetComponent<ParticleEmitter>();
                //minE=pEmitter.minEmission;
                //maxE=pEmitter.maxEmission;

                //pEmitter.minEmission=0;
                //pEmitter.maxEmission=0;

                //particleDuration=pEmitter.maxEnergy;
            }

        }

    }

    // Use this for initialization
    void Start()
    {

    }


    public void Shoot(CharacterBase cur, CharacterBase tgt, SkillClipData skillData, Transform sp)
    {
        srcTower = cur;
        target = tgt;
        skillClipData = skillData;
        shootPoint = sp;
        hit = false;
        hitEffect = "Prefabs/Effect/Explosion";
        _Shoot();
    }


    public void Shoot(CharacterBase tgt, CharacterBase src)
    {
        Shoot(tgt, src, null);
    }

    public void Shoot(CharacterBase tgt, CharacterBase src, Transform sp)
    {
        target = tgt;
        srcTower = src;
        shootPoint = sp;

        hit = false;

        _Shoot();
    }

    public void Shoot(CharacterBase tgt)
    {
        target = tgt;
        hit = false;

        _Shoot();
    }

    public void _Shoot()
    {
        if (type == _ShootObjectType.Projectile) StartCoroutine(ProjectileRoutine());
        else if (type == _ShootObjectType.Missile) StartCoroutine(MissileRoutine());
        else if (type == _ShootObjectType.Beam) StartCoroutine(BeamRoutine());
        //else if(type==_ShootObjectType.Effect) StartCoroutine(EffectRoutine());
    }

    //for rojectile
    public float speed = 1;
    public float maxShootRange = 10;
    public float maxShootAngle = 20;

    //投射程式
    IEnumerator ProjectileRoutine()
    {
        Vector3 targetPos = target.Trans.position;

        thisT.LookAt(target.Trans);
        float angle = Mathf.Min(1, Vector3.Distance(thisT.position, target.Trans.position) / maxShootRange) * maxShootAngle;

        thisT.rotation = thisT.rotation * Quaternion.Euler(Mathf.Clamp(-angle, -42, 42), 0, 0);

        Vector3 startPos = thisT.position;
        float iniRotX = thisT.rotation.eulerAngles.x;

        while (!hit)
        {

            if (target != null && target.ModelObj.activeSelf)
            {
                targetPos = target.Trans.position;
            }

            float currentDist = Vector3.Distance(thisT.position, targetPos);
            if (currentDist < 0.25 && !hit) Hit();

            //calculate ratio of distance covered to total distance
            float totalDist = Vector3.Distance(startPos, targetPos);
            float invR = 1 - currentDist / totalDist;

            //use the distance information to set the rotation, 
            //as the projectile approach target, it will aim straight at the target
            Quaternion wantedRotation = Quaternion.LookRotation(targetPos - thisT.position);
            float rotX = Mathf.LerpAngle(iniRotX, wantedRotation.eulerAngles.x, invR);

            //make y-rotation always face target
            thisT.rotation = Quaternion.Euler(rotX, wantedRotation.eulerAngles.y, wantedRotation.eulerAngles.z);

            //Debug.Log(Time.timeScale+"   "+Time.deltaTime);

            //move forward
            thisT.Translate(Vector3.forward * Mathf.Min(speed * Time.deltaTime, currentDist));

            yield return null;
        }
    }

    //导弹投射
    IEnumerator MissileRoutine()
    {

        Vector3 targetPos = target.Trans.position;

        float randX = Random.Range(-maxShootAngle, 30f);
        float randY = Random.Range(-30f, 30f);
        float randZ = Random.Range(-10f, 10f);

        //make sure the shootObject is facing the target and adjust the projectile angle
        thisT.LookAt(target.Trans);
        float angle = Mathf.Min(1, Vector3.Distance(thisT.position, target.Trans.position) / maxShootRange) * maxShootAngle;
        thisT.rotation = thisT.rotation * Quaternion.Euler(Mathf.Clamp(-angle + randX, -40, 40), randY, randZ);

        Vector3 startPos = thisT.position;
        Quaternion iniRot = thisT.rotation;

        var pool = ObjectPoolManager.Instance.CreatePool<EffectPoolObj>(shootEffect);
        EffectPoolObj effectPoolItem = pool.GetObject();
        effectPoolItem.itemObj.transform.position = thisT.position;
        effectPoolItem.itemObj.transform.localRotation = Quaternion.identity;

        //while the shootObject havent hit the target
        while (!hit)
        {

            if (target != null && target.ModelObj.activeSelf)
            {
                targetPos = target.Trans.position;
            }

            float currentDist = Vector3.Distance(thisT.position, targetPos);
            if (currentDist < 0.25 && !hit) Hit();

            float totalDist = Vector3.Distance(startPos, targetPos);
            float invR = 1 - currentDist / totalDist;

            //use the distance information to set the rotation, 
            //as the projectile approach target, it will aim straight at the target
            Quaternion wantedRotation = Quaternion.LookRotation(targetPos - thisT.position);
            thisT.rotation = Quaternion.Lerp(iniRot, wantedRotation, invR);

            //move forward
            thisT.Translate(Vector3.forward * Mathf.Min(speed * Time.deltaTime, currentDist));

            yield return null;
        }
    }



    IEnumerator BeamRoutine()
    {

        float tempDuration = 0.1f;
        if (srcTower != null) tempDuration = Mathf.Min(srcTower.GetCooldown() * 0.5f, duration);
        else if (target != null) tempDuration = Mathf.Min(srcTower.GetCooldown() * 0.5f, duration);
        if (tempDuration <= 0) tempDuration = Time.deltaTime / 2;

        if (continousDamage) StartCoroutine(BeamRoutineDamage(tempDuration));


        Transform sEffectT = null;

        var pool = ObjectPoolManager.Instance.CreatePool<EffectPoolObj>(shootEffect);
        EffectPoolObj effectPoolItem = pool.GetObject();
        sEffectT = effectPoolItem.itemObj.transform;
        while (tempDuration > 0)
        {
            if (shootPoint != null) thisT.position = shootPoint.position;

            if (sEffectT != null) sEffectT.position = thisT.position;

            tempDuration -= Time.deltaTime;

            if (lineRenderer != null)
            {
                lineRenderer.SetPosition(0, thisT.position);

                float dist = Vector3.Distance(thisT.position, target.Trans.position);
                if (beamLength >= dist)
                {
                    lineRenderer.SetPosition(1, target.Trans.position);
                }
                else
                {
                    Ray ray = new Ray(thisT.position, (target.Trans.position - thisT.position));
                    lineRenderer.SetPosition(1, ray.GetPoint(beamLength));
                }
            }
            else Debug.Log("null");

            if (target.buffSateMgr.HasState(StateType.Death))
            {
                StartCoroutine(Unspawn());
                break;
            }
        }


        yield return null;


        if (!continousDamage) Hit();
    }

    IEnumerator BeamRoutineDamage(float tempDuration)
    {
        int count = (int)Mathf.Max(1, Mathf.Floor(tempDuration / 0.1f));
        int countT = count;

        while (count > 0)
        {
            count -= 1;

            if (count > 0) HitContinous(countT);
            else HitContinousF(countT);

            yield return new WaitForSeconds(0.1f);
        }
    }


    IEnumerator ResetRotation()
    {
        yield return null;
        thisT.rotation = Quaternion.Euler(-90, 0, 0);
    }

    void OnEnable()
    {
        //if(type==_ShootObjectType.Effect){
        //	if(pType==2 && pEmitter!=null){
        //		//Debug.Log("pEmitter");
        //		if(oneShot){
        //			pEmitter.Emit((int)Random.Range(minE, maxE));
        //			StartCoroutine(Disable());
        //		}
        //		else{
        //			pEmitter.minEmission=minE;
        //			pEmitter.maxEmission=maxE;
        //		}
        //	}
        //	else if(pType==1 && pSystem!=null){
        //		//Debug.Log("pSystem");

        //		//thisT.rotation=Quaternion.Euler(90, 0, 0);
        //		StartCoroutine(ResetRotation());

        //		if(!looping){
        //			StartCoroutine(Disable());
        //		}
        //		else{
        //                  var main = pSystem.main;
        //                  main.loop=true;
        //		}
        //	}
        //	else Debug.Log(pType+"  error");
        //}
    }

    public void OnDisable()
    {
        //if(type==_ShootObjectType.Effect){
        //	if(pType==1 && pEmitter!=null){
        //		if(!oneShot){
        //			pEmitter.minEmission=0;
        //			pEmitter.maxEmission=0;
        //		}
        //	}
        //	else if(pType==2 && pSystem!=null){
        //		if(looping){
        //			pSystem.loop=false;
        //		}
        //	}
        //}
    }


    public void HitContinous(int count)
    {
        //if(srcTower != null) srcTower.HitTarget(thisT.position, target, false, count);
        //else if(target != null) target.HitTarget(thisT.position, target, false, count);
    }

    public void HitContinousF(int count)
    {
        //if(hitAudio!=null) AudioManager.PlaySound(hitAudio, thisT.position);
        //if(hitEffect!=null) ObjectPoolManager.Spawn(hitEffect, target.Trans.position, Quaternion.identity);

        //if(srcTower !=null) srcTower.HitTarget(thisT.position, target, true, count);
        //else if(target != null) target.HitTarget(thisT.position, target, true, count);

        StartCoroutine(Unspawn());
    }

    public void Hit()
    {
        hit = true;

        if (hitAudio != null) AudioManager.PlaySound(hitAudio, thisT.position);
        var pool = ObjectPoolManager.Instance.CreatePool<EffectPoolObj>(hitEffect);
        EffectPoolObj effectPoolItem = pool.GetObject();
        this.hitPoolItem = effectPoolItem;
        effectPoolItem.itemObj.SetActive(true);
        effectPoolItem.itemObj.transform.parent = this.target.Trans;
        effectPoolItem.itemObj.transform.position = target.Trans.position;
        effectPoolItem.itemObj.transform.localRotation = Quaternion.identity;

        RoutineComponent routine = srcTower.GetComponent<RoutineComponent>();
        SysAction sysAction = new SysAction(srcTower, target, OperateType.Harm);
        sysAction.skillId = skillClipData.skillCofingId;
        routine.RequestAction(sysAction);
        // this.poolItem.itemObj.SetActive(false);


        StartCoroutine(Unspawn());
    }

    //等待拖尾结束后销毁子弹
    IEnumerator Unspawn()
    {
        yield return new WaitForSeconds(trailDuration + 0.5f);
        if (poolItem != null)
            poolItem.ReturnToPool();
        if (hitPoolItem != null)
            hitPoolItem.ReturnToPool();
        this.enabled = false;
    }

    public IEnumerator Disable()
    {
        yield return new WaitForSeconds(particleDuration + 1);
        if (poolItem != null)
            poolItem.ReturnToPool();
        if (hitPoolItem != null)
            hitPoolItem.ReturnToPool();
    }


}
