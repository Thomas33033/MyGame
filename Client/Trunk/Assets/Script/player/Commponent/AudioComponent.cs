using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioComponent : ComponentBase
{
    public GameObject animationBody;   //动画实体
    private Animation aniBody;         //动画实体

    public AudioClip audioSpawn;
    public AudioClip audioHit;
    public AudioClip audioDead;
    public AudioClip audioScore;

    public AnimationClip[] animationSpawn;
    public AnimationClip[] animationMove;
    public AnimationClip[] animationHit;
    public AnimationClip[] animationDead;
    public AnimationClip[] animationScore;

    public override  void OnInit(CharacterBase p_owner)
    {
        base.OnInit(p_owner);
        if(p_owner.Trans == null)
        {
            return;
        }
        Transform bodyTrans = p_owner.Trans.Find("Body");
        if(bodyTrans != null)
        animationBody = bodyTrans.gameObject;
        if (animationBody != null)
        {
            aniBody = animationBody.GetComponent<Animation>();
            if (aniBody == null) aniBody = animationBody.AddComponent<Animation>();

            if (animationSpawn != null && animationSpawn.Length > 0)
            {
                foreach (AnimationClip clip in animationSpawn)
                {
                    aniBody.AddClip(clip, clip.name);
                    aniBody.GetComponent<Animation>()[clip.name].layer = 1;
                    aniBody.GetComponent<Animation>()[clip.name].wrapMode = WrapMode.Once;
                }
            }

            if (animationMove != null && animationMove.Length > 0)
            {
                foreach (AnimationClip clip in animationMove)
                {
                    aniBody.AddClip(clip, clip.name);
                    aniBody.GetComponent<Animation>()[clip.name].layer = 0;
                    aniBody.GetComponent<Animation>()[clip.name].wrapMode = WrapMode.Loop;
                }
            }

            if (animationHit != null && animationHit.Length > 0)
            {
                foreach (AnimationClip clip in animationHit)
                {
                    aniBody.AddClip(clip, clip.name);
                    aniBody.GetComponent<Animation>()[clip.name].layer = 3;
                    aniBody.GetComponent<Animation>()[clip.name].wrapMode = WrapMode.Once;
                }
            }

            if (animationDead != null && animationDead.Length > 0)
            {
                foreach (AnimationClip clip in animationDead)
                {
                    aniBody.AddClip(clip, clip.name);
                    aniBody.GetComponent<Animation>()[clip.name].layer = 3;
                    aniBody.GetComponent<Animation>()[clip.name].wrapMode = WrapMode.Once;
                }
            }

            if (animationScore != null && animationScore.Length > 0)
            {
                foreach (AnimationClip clip in animationScore)
                {
                    aniBody.AddClip(clip, clip.name);
                    aniBody.GetComponent<Animation>()[clip.name].layer = 3;
                    aniBody.GetComponent<Animation>()[clip.name].wrapMode = WrapMode.Once;
                }
            }
        }

    }

    public override void OnUpdate(float dt)
    {
        base.OnUpdate(dt);
    }
	
}
