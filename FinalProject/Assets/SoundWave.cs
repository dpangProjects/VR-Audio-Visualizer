using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundWave : MonoBehaviour
{
    const int SAMPLE_SIZE = 1024;
    public float rmsValue;
    public float dbValue;
    public float pitchValue;
    
    public float visualModifier = 100.0f;
    public float smoothSpeed = 10.0f;
    public float keepPercentage = 0.5f;

    public Transform[] visualList;
    public float[] visualScale;
    private int amnVisual = 64;

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

        Vector3 center = Vector3.zero;
        float radius = 10.0f;

        for (int i = 0; i<amnVisual; i++) {
            float ang = i*1.0f/amnVisual;
            ang = ang *Mathf.PI*2;

            float x = center.x+Mathf.Cos(ang)*radius;
            float y = center.y + Mathf.Sin(ang) * radius;

            Vector3 pos = center +new Vector3(x,y,0);
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            go.transform.position = pos;
            go.transform.rotation = Quaternion.LookRotation(Vector3.forward, pos);
            visualList[i] = go.transform;
        }
    }



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

            visualList[visualIndex].localScale = Vector3.one + Vector3.up * visualScale[visualIndex];
            visualIndex++;
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
}
