using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Transform camTransform;

    public AnimationCurve jumpPauseCurve;
    public Transform leftHand, rightHand;

    void Awake()
    {
        camTransform = Camera.main.transform;

    }

    void Update()
    {
        CheckFallToDeath();
    }
    void CheckFallToDeath()
    {
        if (transform.position.y < -100)
        {
            //LevelManager.instance.audioMng.Play("falldead");
            StartCoroutine(LevelManager.instance.DieAfter(0));
        }
    }

    IEnumerator JumpShake(float duration, float magnitude)
    {
        Vector3 OriginalPos = camTransform.localPosition;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {

            camTransform.localPosition = OriginalPos ;

            camTransform.localPosition = OriginalPos
                + Vector3.down * magnitude * jumpPauseCurve.Evaluate(elapsed / duration);

            elapsed += Time.deltaTime;
            yield return null;
        }
        camTransform.localPosition = OriginalPos;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("Player hit " + hit.gameObject.tag);
        if (hit.gameObject.tag == "cactus")
        {
            LevelManager.instance.audioMng.Play("dead");
            StartCoroutine(LevelManager.instance.DieAfter(1));
        }
        else if (hit.gameObject.tag == "win")
        {
            if (LevelManager.instance.IsPortalOn())
            {
                LevelManager.instance.audioMng.Play("sfx_victory");
                //LevelManager.instance.audioMng.Stop("bg");
                StartCoroutine(LevelManager.instance.DieAfter(4)); // 23
            }
        }
        else if (hit.gameObject.tag == "pickup_dj")
        {
            // sfx double jump pickup collected
            LevelManager.instance.audioMng.Play("sfx_dj_picked");
            //canDoubleJump = true;
            Destroy(hit.gameObject);
        }
        else if (hit.gameObject.tag == "pickup_sp")
        {
            // sfx double jump pickup collected
            LevelManager.instance.audioMng.Play("sfx_sp_picked");
            LevelManager.instance.ObtainedKey();// add score
            Destroy(hit.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // if this is pickup, set player picked up
        if (other.gameObject.tag == "pickup_sp")
        {
            // sfx double jump pickup collected
            LevelManager.instance.audioMng.Play("sfx_sp_picked");
            LevelManager.instance.ObtainedKey();// add score
            Destroy(other.gameObject);
        }
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.DrawSphere(transform.position + Vector3.up*footYoffset, sphereRadius);
    //}
}
