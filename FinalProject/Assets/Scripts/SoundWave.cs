using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class SoundWave : MonoBehaviour
{
    public VisualEffect firework;
    public Material purpleNeon;
    public Material greenNeon;

    public float timing;
    private bool change = false;
    public float scalar = 100.0f;

    const int SAMPLE_SIZE = 1024;
    public float rmsValue;
    public float dbValue;
    public float pitchValue;
    
    public float maxVisualScale = 25.0f;
    public float visualModifier = 100.0f;
    public float smoothSpeed = 10.0f;
    public float keepPercentage = 0.5f;

    public GameObject[] gos;
    public Transform[] visualList;
    public float[] visualScale;
    public int amnVisual = 64;

    private AudioSource source;
    private float[] samples;
    private float[] spectrum;
    private float sampleRate;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        samples = new float[SAMPLE_SIZE];
        spectrum = new float[SAMPLE_SIZE];
        sampleRate = AudioSettings.outputSampleRate;

        // SpawnLine();
        SpawnCircle();
    }

    private void SpawnLine() {
        visualScale = new float[amnVisual];
        visualList = new Transform[amnVisual];

        for (int i = 0; i<amnVisual; i++) {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            go.GetComponent<Renderer>().material.color = new Color(255,0,0);
            visualList[i] = go.transform;
            visualList[i].position = Vector3.right*i;
        }
    }
    private void SpawnCircle(){
        visualScale = new float[amnVisual];
        visualList = new Transform[amnVisual];
        gos = new GameObject[amnVisual];

        Vector3 center = transform.position;
        float radius = 10.0f;

        for (int i = 0; i<amnVisual; i++) {
            float ang = i*1.0f/amnVisual;
            ang = ang *Mathf.PI*2;

            float x = Mathf.Sin(ang)*radius;
            float y = Mathf.Cos(ang) * radius;

            Vector3 pos = center + new Vector3(x,0,y);
            print(pos);
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            go.GetComponent<MeshRenderer>().material = purpleNeon;
            go.transform.position = pos;
            go.transform.rotation = Quaternion.LookRotation(Vector3.forward, pos);
            visualList[i] = go.transform;
            gos[i] = go;
        }
    }

    float elapsed = 0f;

    // Update is called once per frame
    void Update()
    {
        AnalyzeSound();
        UpdateVisual();
        
    }

    private void UpdateVisual() {
        int visualIndex = 0;
        int spectrumIndex = 0;
        int averageSize = (int)(SAMPLE_SIZE*keepPercentage
        )/amnVisual;
        float scalesum = 0;
        while (visualIndex < amnVisual) {
            int j = 0;
            float sum = 0;
            while (j< averageSize) {
                sum += spectrum[spectrumIndex];
                spectrumIndex++;
                j++;
            }

            float scaleY = sum/averageSize*visualModifier;
            visualScale[visualIndex] -= Time.deltaTime * smoothSpeed;
            if(visualScale[visualIndex] < scaleY)
                visualScale[visualIndex] = scaleY;
            
            if(visualScale[visualIndex] > maxVisualScale)
                visualScale[visualIndex] = maxVisualScale;

            visualList[visualIndex].localScale = Vector3.one + Vector3.right * visualScale[visualIndex];
            scalesum+=visualList[visualIndex].localScale.x;
            visualIndex++;

        }
        elapsed += Time.deltaTime;
        if (elapsed >= timing && scalesum > scalar) {
            elapsed = elapsed % timing;
            changeMaterial(gos);
            firework.Play();
        }
    }

    private void AnalyzeSound() {
        source.GetOutputData(samples, 0);

        int i = 0;
        float sum = 0;
        for (; i < SAMPLE_SIZE; i++) {
            sum = samples[i] * samples[i];
        }
        rmsValue = Mathf.Sqrt(sum/SAMPLE_SIZE);

        dbValue = 20*Mathf.Log10(rmsValue/0.1f);
        source.GetSpectrumData(spectrum,0,FFTWindow.BlackmanHarris);

        float maxV = 0;
        var maxN = 0;
        for (i=0; i < SAMPLE_SIZE; i++){ // find max 
            if (spectrum[i] > maxV && spectrum[i] > 0.0f){
                continue;
             }
             maxV = spectrum[i];
             maxN = i;
        }
        float freqN = maxN; // pass the index to a float variable
        if (maxN > 0 && maxN < SAMPLE_SIZE-1){ // interpolate index using neighbours
            var dL = spectrum[maxN-1]/spectrum[maxN];
            var dR = spectrum[maxN+1]/spectrum[maxN];
            freqN += 0.5f*(dR*dR - dL*dL);
        }
     pitchValue = freqN*(sampleRate/2)/SAMPLE_SIZE; // convert index to frequency
    
    
    }

    private void changeMaterial(GameObject[] gos) {
        int visualIndex = 0;
        while (visualIndex < amnVisual) {
            if (change) {
                gos[visualIndex].GetComponent<MeshRenderer>().material = greenNeon;
            }
            else {
                gos[visualIndex].GetComponent<MeshRenderer>().material = purpleNeon;    
            }
            visualIndex++;
        }
        change = !change;
    }

    private void shootFirework() {

    }
}
