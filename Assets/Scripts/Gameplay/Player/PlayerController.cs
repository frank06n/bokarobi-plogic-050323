using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMove playerMove;
    private bool dead = false;

    void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
    }

    private void FixedUpdate()
    {
        if (dead) return;

        Vector3 sp1 = transform.position + Vector3.up * 0.5f;
        Vector3 sp2 = transform.position + Vector3.down * 0.5f;
        if (Physics.CheckCapsule(sp1, sp2, 0.5f, LevelManager.instance.obstacles_layer))
        {
            SetDead();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (dead) return; 
        
        if (other.CompareTag(Interactable.Pickup_Key))
        {
            if (other.GetComponent<PickupLogic>().TryToCollect())
            {
                AudioManager.instance.Play(Audio.PICKUP_SUPOINT);
                LevelManager.instance.OnPlayerCollectKey();
                Destroy(other.gameObject);
            }
        }
        else if (other.CompareTag(Interactable.Pickup_DoubleJump))
        {
            if (other.GetComponent<PickupLogic>().TryToCollect())
            {
                AudioManager.instance.Play(Audio.PICKUP_DJ);
                playerMove.SetCanDoubleJump();
                Destroy(other.gameObject);
            }
        }
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (dead) return;

        GameObject hitObject = hit.gameObject;

        if (hitObject.CompareTag(Interactable.Cactus))
        {
            SetDead();
        }
        else if (hitObject.CompareTag(Interactable.DeathY))
        {
            SetDead();
            Destroy(hitObject);
        }
        else if (hitObject.CompareTag(Interactable.WinOrb))
        {
            if (LevelManager.instance.finishPortal.IsOn())
            {
                LevelManager.instance.finishPortal.DestroyEntryCollider();

                AudioManager.instance.Play(Audio.PORTAL_ENTERED);
                SetWon();
            }
        }
    }

    private void SetDead()
    {
        dead = true;
        playerMove.SetImmobile(1f);
        AudioManager.instance.Play(Audio.DEATH);

        bool won = false;
        LevelManager.instance.SetGameOver(won);
    }

    private void SetWon()
    {
        playerMove.SetImmobile(1f);
        bool won = true;
        LevelManager.instance.SetGameOver(won);
    }
}
