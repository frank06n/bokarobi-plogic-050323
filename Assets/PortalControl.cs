using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalControl : MonoBehaviour
{
    Animator spinAnim;
    AudioSource portalSfx;

    [SerializeField] GameObject portalPS_obj;
    [SerializeField] GameObject portal_obj;
    [SerializeField] GameObject portalLight_obj;

    [SerializeField] Material portalLitMat;
    [SerializeField] Material portalUnlitMat;

    // Start is called before the first frame update
    void Awake()
    {
        spinAnim = GetComponent<Animator>();
        portalSfx = GetComponent<AudioSource>();
        spinAnim.speed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            TurnOn();
        }
    }

    public void TurnOn()
    {
        portalPS_obj.SetActive(true);
        portalLight_obj.SetActive(true);
        StartCoroutine(TogglePortalSpin(5f, true));
        portal_obj.GetComponent<Renderer>().material = portalLitMat;
        portalSfx.Play();
    }

    private IEnumerator TogglePortalSpin(float duration, bool start)
    {
        float frac = 0;
        while (frac < 1)
        {
            frac += Time.deltaTime / duration;
            if (frac > 1f) frac = 1f;

            spinAnim.speed = start ? frac : (1 - frac);
            yield return null;
        }
    }
}
