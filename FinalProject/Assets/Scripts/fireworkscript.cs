using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class fireworkscript : MonoBehaviour
{
    public bool play =true;
    public VisualEffect effect;
    // Start is called before the first frame update
    void Start()
    {
        effect = GetComponent<VisualEffect>();
        effect.Play();
    }

    // Update is called once per frame
    void Update()
    {
    if(Input.GetKeyDown(KeyCode.Space) && play==true){
        effect.Play();
        play=!play;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && play==false) {
            effect.Stop();
            play=!play;
        }
    }
}
