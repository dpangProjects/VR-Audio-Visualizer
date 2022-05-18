using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colorChange : MonoBehaviour
{
    public AudioSource audioSource;
    public float updateInterval = 0.1f;
    public int sampleDataLength = 1024;
    public float clipLoudness;
    private float[] clipSampleData;
    private float currentUpdateTime = 0f;
    public float threshold=1;
    public GameObject origin;

    // Color Stuff below this comment
    private List<GameObject> changes;
    private List<List<Material>> ogMaterials;

    private void Awake() {
        clipSampleData = new float[sampleDataLength];
    }
    
    private void Start()
    {
        changes = new List<GameObject> (GameObject.FindGameObjectsWithTag("change")); // A list of all the object parts, I gave the tag "change"
        ogMaterials = new List<List<Material>>(); // A list of lists where each inside list is the materials of each object part

        foreach (GameObject change in changes) { // for each part of the object, get all the materials it is made up of 
            List<Material> tempList = new List<Material> (change.GetComponent<Renderer>().materials);
            ogMaterials.Add(tempList);
        }
    }

    private void Update()
    {
        currentUpdateTime += Time.deltaTime;
        if (currentUpdateTime >= updateInterval) {
            currentUpdateTime = 0f;
            audioSource.clip.GetData(clipSampleData, audioSource.timeSamples);
            clipLoudness = 0f;
            foreach (var sample in clipSampleData) {
                clipLoudness += Mathf.Abs(sample);
            }
            if (clipLoudness <= threshold) {
                foreach (GameObject change in changes) {
                    foreach (Material mat in change.GetComponent<Renderer>().materials) {
                        mat.SetColor("_BaseColor", Color.black);
                        // changes[i].GetComponent<Renderer>().material = darkMaterial;
                    }
                }
            } else {
                // print("CHANGE COLOR!!");
                foreach (GameObject change in changes) {
                    for (int i = 0; i < change.GetComponent<Renderer>().materials.Length; i++) {
                        // print("Changing" + change.GetComponent<Renderer>().materials[i].name);
                        // print("OG mat is" + ogMaterials[changes.IndexOf(change)][i].name);
                        // change.GetComponent<Renderer>().materials[i] = ogMaterials[changes.IndexOf(change)][i];
                        change.GetComponent<Renderer>().materials[i].SetColor("_BaseColor", Color.green);
                    }
                }
            }
        }
        // print(clipLoudness);
    }
}
