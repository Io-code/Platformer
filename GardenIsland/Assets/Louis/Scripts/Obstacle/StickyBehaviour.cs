using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyBehaviour : MonoBehaviour
{
    public bool permanent;

    public float stickyRepopTime;
    public float stickyTime;

    public float warningBuffer;

    bool sticky = true;
    bool stickyShow = true;

    bool cycling = true;

    public Material stickyMaterial, normalMaterial;
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
                StartCoroutine(SpikeCycle((sticky) ? stickyTime : stickyRepopTime));
        }
        else
        {
            sticky = stickyShow = true;
        }
    }
    private void Update()
    {
        rend.sharedMaterial = (stickyShow) ? stickyMaterial : normalMaterial;
        tag = (sticky) ? "Stickable" : "Untagged";
    }
    public IEnumerator SpikeCycle(float time)
    {
        cycling = false;

        if (sticky)
        {
            yield return new WaitForSeconds(time - warningBuffer);
            stickyShow = false;
        }

        yield return new WaitForSeconds((!sticky) ? time : warningBuffer);
        sticky = !sticky;

        if (stickyShow != sticky)
            stickyShow = sticky;

        cycling = true;
    }
}
