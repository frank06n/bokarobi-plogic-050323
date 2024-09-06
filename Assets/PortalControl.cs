using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalControl : MonoBehaviour
{
    Animator spinAnim;
    AudioSource portalSfx;
    bool turnedOn = false;

    [SerializeField] GameObject portalPS_obj;
    [SerializeField] GameObject portal_obj;
    [SerializeField] GameObject portalLight_obj;
    [SerializeField] GameObject portalEntryCollider_obj;

    [SerializeField] Material portalLitMat;
    [SerializeField] Material portalUnlitMat;

    // Start is called before the first frame update
    void Awake()
    {
        spinAnim = GetComponent<Animator>();
        portalSfx = GetComponent<AudioSource>();
        spinAnim.speed = 0;
    }

    public void TurnOn()
    {
        turnedOn = true;
        portalPS_obj.SetActive(true);
        portalLight_obj.SetActive(true);
        portalEntryCollider_obj.SetActive(true);
        StartCoroutine(TogglePortalSpin(5f, true));
        portal_obj.GetComponent<Renderer>().material = portalLitMat;
        portalSfx.Play();
    }

    public bool IsOn()
    {
        return turnedOn;
    }

    public void DestroyEntryCollider()
    {
        Destroy(portalEntryCollider_obj);
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
