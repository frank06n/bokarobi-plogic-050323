using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMove playerMove;
    private const float YDeathThreshold = -100;
    private bool dead = false;

    void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
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

                AudioManager.instance.Play(Audio.M_VICTORY);
                SetWon();
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
            LevelManager.instance.OnPlayerCollectKey();
            Destroy(hitObject);
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
