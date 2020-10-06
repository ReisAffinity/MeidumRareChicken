﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnCollision : MonoBehaviour
{
    public AudioSource soundToPlay;
    public int groundLayerNo = 8;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        
        if (other.gameObject.layer != groundLayerNo)
        {
            //Just in case two cars hits at the same time.
            soundToPlay.Stop();

            soundToPlay.pitch = Random.Range(0.8f, 1.2f);

            soundToPlay.Play();
        }
    }
}
