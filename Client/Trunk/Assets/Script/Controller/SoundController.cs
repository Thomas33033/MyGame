using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimClipData {
    public float soundStartTime;
    public string soundName;
    public string clipName;
}

public class SoundController : Singleton<SoundController>
{
    public Dictionary<string, AudioClip> audioClipMap = new Dictionary<string, AudioClip>();
    public void AddEvent(Animator animator, AnimClipData clipData)
    {
        SoundEventListener listener = animator.gameObject.GetComponent<SoundEventListener>();
        if (listener)
        {
            animator.gameObject.GetComponent<SoundEventListener>();
        }

        AnimationEvent aniEvent = new AnimationEvent();
        aniEvent.time = clipData.soundStartTime;
        aniEvent.stringParameter = clipData.soundName;
        aniEvent.functionName = "PlaySound";
        RuntimeAnimatorController contrl = animator.runtimeAnimatorController;
        for (int i = 0; i < contrl.animationClips.Length; i++)
        {
            AnimationClip clip = contrl.animationClips[i];
            
            if (clip.name == clipData.clipName)
            {
                clip.AddEvent(aniEvent);
            }
        }
    }

    public void ClearEvent(Animator animator)
    {
        RuntimeAnimatorController contrl = animator.runtimeAnimatorController;
        for (int i = 0; i < contrl.animationClips.Length; i++)
        {
            AnimationClip clip = contrl.animationClips[i];
            clip.events = new AnimationEvent[0];
        }
    }


    public void PlaySound(string name,Vector3 pos)
    {
        if (!audioClipMap.ContainsKey(name))
        {
            AudioClip clip = Resources.Load<AudioClip>("Audio/"+name);
            audioClipMap.Add(name, clip);
        }

        AudioManager.PlaySound(audioClipMap[name], pos);


    }
}
