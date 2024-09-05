using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMove playerMove;
    private const float YDeathThreshold = -100;

    void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
    }

    void Update()
    {
        CheckFallenToDeath();
    }
    void CheckFallenToDeath()
    {
        if (transform.position.y < YDeathThreshold)
        {
            SetDead();
        }
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        GameObject hitObject = hit.gameObject;

        if (hitObject.CompareTag(Interactable.Cactus))
        {
            SetDead();
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
        bool won = false;
        playerMove.SetImmobile(1f);
        LevelManager.instance.SetGameOver(won);
    }

    private void SetWon()
    {
        bool won = true;
        playerMove.SetImmobile(1f);
        LevelManager.instance.SetGameOver(won);
    }
}
