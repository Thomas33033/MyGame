using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitCreep : Unit {

	public float moveSpeed=3;
	public bool immuneToSlow=false; //减速免疫
	
	public bool flying=false;           //是否飞行
	public float flightHeightOffset=3f; //飞行高度偏移
	
	public int[] value=new int[1];
	
	public GameObject spawnEffect;   //出生特效
	public GameObject deadEffect;    //死亡特效
	public GameObject scoreEffect;   //得分特效
	
	public GameObject animationBody;   //动画实体
	private Animation aniBody;         //动画实体
	public AnimationClip[] animationSpawn;  
	public AnimationClip[] animationMove;
	public AnimationClip[] animationHit;
	public AnimationClip[] animationDead;
	public AnimationClip[] animationScore;
	public float moveAnimationModifier=1.0f; //移动动画调节器
	
	public AudioClip audioSpawn;
	public AudioClip audioHit;
	public AudioClip audioDead;
	public AudioClip audioScore;
	
	[HideInInspector] public Vector3 dynamicOffset;  //动态偏移
	
	[HideInInspector] public int waveID;              //波次ID
	
	public GameObject spawnUponDestroyed;             //繁衍在死亡之上
	public int spawnNumber;                          //繁衍数量
	

	public override void Awake () {
		base.Awake();
		
		SetSubClassInt(this);
		
		if(thisObj.GetComponent<Collider>()==null){
			SphereCollider col=thisObj.AddComponent<SphereCollider>();
			col.center=new Vector3(0, 0.0f, 0);
			col.radius=0.25f;
		}
		
		if(animationBody!=null){
			aniBody=animationBody.GetComponent<Animation>();
			if(aniBody==null) aniBody=animationBody.AddComponent<Animation>();
			
			if(animationSpawn!=null && animationSpawn.Length>0){
				foreach(AnimationClip clip in animationSpawn){
					aniBody.AddClip(clip, clip.name);
					aniBody.GetComponent<Animation>()[clip.name].layer=1;
					aniBody.GetComponent<Animation>()[clip.name].wrapMode=WrapMode.Once;
				}
			}
			
			if(animationMove!=null && animationMove.Length>0){
				foreach(AnimationClip clip in animationMove){
					aniBody.AddClip(clip, clip.name);
					aniBody.GetComponent<Animation>()[clip.name].layer=0;
					aniBody.GetComponent<Animation>()[clip.name].wrapMode=WrapMode.Loop;
				}
			}
			
			if(animationHit!=null && animationHit.Length>0){
				foreach(AnimationClip clip in animationHit){
					aniBody.AddClip(clip, clip.name);
					aniBody.GetComponent<Animation>()[clip.name].layer=3;
					aniBody.GetComponent<Animation>()[clip.name].wrapMode=WrapMode.Once;
				}
			}
			
			if(animationDead!=null && animationDead.Length>0){
				foreach(AnimationClip clip in animationDead){
					aniBody.AddClip(clip, clip.name);
					aniBody.GetComponent<Animation>()[clip.name].layer=3;
					aniBody.GetComponent<Animation>()[clip.name].wrapMode=WrapMode.Once;
				}
			}
			
			if(animationScore!=null && animationScore.Length>0){
				foreach(AnimationClip clip in animationScore){
					aniBody.AddClip(clip, clip.name);
					aniBody.GetComponent<Animation>()[clip.name].layer=3;
					aniBody.GetComponent<Animation>()[clip.name].wrapMode=WrapMode.Once;
				}
			}
		}
		
		//if(spawnEffect!=null) ObjectPoolManager.New(spawnEffect, 5, false);
		//if(deadEffect!=null) ObjectPoolManager.New(deadEffect, 5, false);
	}

	public override void Start () {
		if(!flying) thisObj.layer=LayerManager.LayerCreep();
		else thisObj.layer=LayerManager.LayerCreepF();
		
		base.Start();
	}
	
	public void SetMoveSpeed(float moveSpd){
		moveSpeed=moveSpd;
	}
	
	
	//init a unitCreep, give it a list of Vector3 point as the path it should follow
	public void Init(List<Vector3> waypoints, int wID){
		base.Init();
		waveID=wID;
		wp=waypoints;
		wpMode=true;
		
		if(flying) thisT.position+=new Vector3(0, flightHeightOffset, 0);
		
		currentMoveSpd=moveSpeed;
		
		if(aniBody!=null && animationMove!=null && animationMove.Length>0){
			foreach(AnimationClip clip in animationMove){
				aniBody.GetComponent<Animation>()[clip.name].speed=currentMoveSpd*moveAnimationModifier;
			}
		}
		
		float allowance=BuildManager.GetGridSize();
		dynamicOffset=new Vector3(Random.Range(-allowance, allowance), 0, Random.Range(-allowance, allowance));
		thisT.position+=dynamicOffset;
		
		PlaySpawn();
		PlayMove();
	}
	
	//init a unitCreep, give it a path instance so it can retirve the path
	public void Init(GamePath p, int wID){
		base.Init();
		waveID=wID;
		path=p;
		wpMode=false;
		
		if(flying) thisT.position+=new Vector3(0, flightHeightOffset, 0);
		
		currentMoveSpd=moveSpeed;
		
		if(aniBody!=null && animationMove!=null && animationMove.Length>0){
			foreach(AnimationClip clip in animationMove){
				aniBody.GetComponent<Animation>()[clip.name].speed=currentMoveSpd*moveAnimationModifier;
			}
		}
		
		//if using dynamic wap pos, set an offset based on gridsize
		if(path.dynamicWP>0){
			//float allowance=BuildManager.GetGridSize()*0.35f;
			float allowance=path.dynamicWP;
			dynamicOffset=new Vector3(Random.Range(-allowance, allowance), 0, Random.Range(-allowance, allowance));
			thisT.position+=dynamicOffset;
		}
		else dynamicOffset=Vector3.zero;
		
		//if(spawnEffect!=null) ObjectPoolManager.Spawn(spawnEffect, thisT.position, Quaternion.identity);
		PlaySpawn();
		PlayMove();
	}
	
	public void Dead()
    {

		//if(deadEffect!=null) ObjectPoolManager.Spawn(deadEffect, thisT.position, Quaternion.identity);
		float duration=PlayDead();
		StartCoroutine(Unspawn(duration));
		
		//spawn more unit if there's one assigned
		if(spawnUponDestroyed!=null){
			
			//SpawnManager.AddActiveUnit(waveID, spawnNumber);
			
			for(int i=0; i<spawnNumber; i++){
				//generate a small offset position within the grid size so not all creep spawned on top of each other and not too far apart
				float allowance=BuildManager.GetGridSize()/2;
				
				float x=Random.Range(-allowance, allowance);
				float y=Random.Range(-allowance, allowance);
				
				Vector3 pos=thisT.position+new Vector3(x, 0, y);
                //GameObject obj=ObjectPoolManager.Spawn(spawnUponDestroyed, pos, thisT.rotation);
				
                //UnitCreep unit=obj.GetComponent<UnitCreep>();
                //unit.Init(path, waveID);
                ////resume the path currently followed by this unit
                //StartCoroutine(unit.ResumeParentPath(wpMode, wp, wpCounter, currentPS, subPath, currentPathID, subWPCounter));
			}
		}
	}
	
	public void Score(){
        //if(scoreEffect!=null) ObjectPoolManager.Spawn(scoreEffect, thisT.position, Quaternion.identity);
        //float duration=PlayScore();
		
        //StartCoroutine(Unspawn(duration));
	}
	
	public override void Update () {
		base.Update();
		
		//~ if(GetWPCounter()==wp.Count){
			//~ //final waypoint reached
			//~ WaypointReachEvent();
			//~ ObjectPoolManager.Unspawn(thisObj);
		//~ }
	}
	
	public void Stunned(){
		if(aniBody!=null){
			if(animationMove!=null && animationMove.Length>0){
				for(int i=0; i<animationMove.Length; i++){
					aniBody.Stop(animationMove[i].name);
				}
			}
		}
	}
	
	public void Unstunned(){
		PlayMove();
	}
	
	private AnimationClip[] animationAttack;
	private AnimationClip animationIdle;
	
	public void SetAttackAnimation(AnimationClip[] aniAttack){
		animationAttack=aniAttack;
		
		if(aniBody!=null && animationAttack!=null && animationAttack.Length>0){
			foreach(AnimationClip clip in animationHit){
				aniBody.AddClip(clip, clip.name);
				aniBody.GetComponent<Animation>()[clip.name].layer=5;
				aniBody.GetComponent<Animation>()[clip.name].wrapMode=WrapMode.Once;
			}
		}
	}
	
	public void SetIdleAnimation(AnimationClip aniIdle){
		animationIdle=aniIdle;
		if(aniBody!=null){
			aniBody.AddClip(animationIdle, animationIdle.name);
			aniBody.GetComponent<Animation>()[animationIdle.name].layer=-1;
			aniBody.GetComponent<Animation>()[animationIdle.name].wrapMode=WrapMode.Loop;
		}
	}
	
	public void StopAnimation(){
		if(aniBody!=null) aniBody.Stop();
		
		if(aniBody!=null && animationIdle!=null){
			aniBody.Play(animationIdle.name);
		}
	}
	
	public void ResumeAnimation(){
		PlayMove();
	}
	
	public bool PlayAttack(){
		if(aniBody!=null && animationAttack!=null && animationAttack.Length>0){
			aniBody.CrossFade(animationAttack[Random.Range(0, animationAttack.Length-1)].name);
			return true;
		}
		return false;
	}
	
	public void PlayMove(){
		if(aniBody!=null && animationMove!=null && animationMove.Length>0){
			aniBody.Play(animationMove[Random.Range(0, animationMove.Length-1)].name);
		}
	}
	
	public void PlaySpawn(){
		if(aniBody!=null && animationSpawn!=null && animationSpawn.Length>0){
			aniBody.CrossFade(animationSpawn[Random.Range(0, animationSpawn.Length-1)].name);
		}
		
		if(audioSpawn!=null) AudioManager.PlaySound(audioSpawn, thisT.position);
	}
	
	public void PlayHit(){
		//Debug.Log("Play hit");
		if(aniBody!=null && animationHit!=null && animationHit.Length>0){
			aniBody.CrossFade(animationHit[Random.Range(0, animationHit.Length-1)].name);
		}
		
		if(audioHit!=null) AudioManager.PlaySound(audioHit, thisT.position);
	}
	
	public float PlayDead(){
		float duration=0;
		
		if(aniBody!=null){
			aniBody.Stop();
		}
		
		if(aniBody!=null && animationDead!=null && animationDead.Length>0){
			int rand=Random.Range(0, animationDead.Length-1);
			aniBody.CrossFade(animationDead[rand].name);
			duration=animationDead[rand].length;
		}
		
		if(audioDead!=null){
			AudioManager.PlaySound(audioDead, thisT.position);
			duration=Mathf.Max(audioDead.length, duration);
		}
		
		return duration;
	}
	
	public float PlayScore(){
		float duration=0;
		
		if(aniBody!=null && animationScore!=null && animationScore.Length>0){
			int rand=Random.Range(0, animationDead.Length-1);
			aniBody.CrossFade(animationScore[rand].name);
			duration=animationScore[rand].length;
		}
		
		if(audioScore!=null) {
			AudioManager.PlaySound(audioScore, thisT.position);
			duration=Mathf.Max(audioScore.length, duration);
		}
		
		return duration;
	}
	
	IEnumerator Unspawn(float duration){
		yield return new WaitForSeconds(duration);
		//ObjectPoolManager.Unspawn(thisObj);
	}

}
