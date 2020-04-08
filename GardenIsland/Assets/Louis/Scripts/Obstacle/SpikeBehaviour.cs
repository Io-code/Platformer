using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeBehaviour : MonoBehaviour
{
    public bool permanent;

    public float spikeRepopTime;
    public float spikeTime;

    public float warningBuffer;

    bool spike = true;
    bool spikeShow = true;

    bool cycling = true;

    public Material spikeMaterial, normalMaterial;
    Renderer rend;

    // Start is called before the first frame update
    private void Start()
    {
        rend = GetComponent<Renderer>();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (!permanent)
        {
            if (cycling)
                StartCoroutine(SpikeCycle((spike) ? spikeTime : spikeRepopTime));
        }
        else
        {
            spike = spikeShow = true;
        }
    }
    private void Update()
    {
        rend.sharedMaterial = (spikeShow) ? spikeMaterial : normalMaterial;
        tag = (spike) ? "Spike" : "Untagged";
    }
    public IEnumerator SpikeCycle(float time)
    {
        cycling = false;

        if (!spike)
        {
            yield return new WaitForSeconds(time - warningBuffer);
            spikeShow = true;
        }
            
        yield return new WaitForSeconds((spike)?time:warningBuffer); 
        spike = !spike;

        if (spikeShow != spike)
            spikeShow = spike;
        
        cycling = true;
    }
 
}
