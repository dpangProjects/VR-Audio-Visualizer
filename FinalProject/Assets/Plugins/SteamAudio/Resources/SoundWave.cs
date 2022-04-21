using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    const int SAMPLE_SIZE = 1024;
    public float rmsValue;
    public float dbValue;
    public float pitchValue;

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
    }

    // Update is called once per frame
    void Update()
    {
        AnalyzeSound();
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
