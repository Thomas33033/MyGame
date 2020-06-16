using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMoveAnimTest : MonoBehaviour
{

    public ParticleRippleCoins particelCoins;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayCoinsAnimation()
    {
        particelCoins.rippleInit();
    }

}
