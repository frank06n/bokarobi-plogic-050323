using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMove playerMove;
    private FinishPortalLogic finishPortal;
    private readonly float YDeathThreshold = -100;

    void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        finishPortal = FindObjectOfType<FinishPortalLogic>();
    }

    void Update()
    {
        CheckFallenToDeath();
    }
    void CheckFallenToDeath()
    {
        if (transform.position.y < YDeathThreshold)
        {
            //PlaySfx("falldead");
            DieAfter(0);
        }
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        GameObject hitObject = hit.gameObject;

        if (hitObject.CompareTag(Interactable.Cactus))
        {
            //play dead sfx
            DieAfter(1);
        }
        else if (hitObject.CompareTag(Interactable.WinOrb))
        {
            if (finishPortal.isOn())
            {
                AudioManager.instance.Play(Audio.M_VICTORY);
                //LevelManager.instance.audioMng.Stop("bg");
                DieAfter(4);
            }
        }
        else if (hitObject.CompareTag(Interactable.Pickup_DoubleJump))
        {
            AudioManager.instance.Play(Audio.DOUBLEJUMP_PICKED);
            playerMove.SetCanDoubleJump();
            Destroy(hitObject);
        }
        else if (hitObject.CompareTag(Interactable.Pickup_Key))
        {
            AudioManager.instance.Play(Audio.KEY_PICKED);
            LevelManager.instance.OnPlayerCollectKey();// add score
            Destroy(hitObject);
        }
    }

    private void DieAfter(float time)
    {
        StartCoroutine(LevelManager.instance.DieAfter(time));
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.DrawSphere(transform.position + Vector3.up*footYoffset, sphereRadius);
    //}
}
