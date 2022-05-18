using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playaudio : MonoBehaviour
{
    private bool playing = true;
    public AudioSource drums;
    public AudioSource bass;
    public AudioSource vocals;
    public AudioSource chorus;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            drums.mute = !drums.mute;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            bass.mute = !bass.mute;
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            vocals.mute = !vocals.mute;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            chorus.mute = !chorus.mute;
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
           if(playing){
                  drums.Pause();
                  bass.Pause();
                  vocals.Pause();
                  chorus.Pause();
                  playing = !playing;
        }
            else {
                  drums.Play();
                  bass.Play();
                  vocals.Play();
                  chorus.Play();
                  playing = !playing;      
            }   
        }
    }
}
