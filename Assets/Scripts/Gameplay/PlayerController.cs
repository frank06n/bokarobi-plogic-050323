using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMove pmv;

    void Awake()
    {
        pmv = GetComponent<PlayerMove>();
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
            pmv.SetDoubleJump(true);
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
