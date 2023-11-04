using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class Interactable
{
    public static readonly string Cactus = "cactus";
    public static readonly string WinOrb = "win";
    public static readonly string Pickup_Key = "pickup_sp";
    public static readonly string Pickup_DoubleJump = "pickup_dj";
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public float gravity;
    public LayerMask ground;

    [HideInInspector] public bool gameOver;

    private int collectedKeys;
    private int totalKeys;
    private FinishPortalLogic finishPortal;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;

        finishPortal = FindObjectOfType<FinishPortalLogic>();

        gameOver = false;

        totalKeys = GameObject.FindGameObjectsWithTag("pickup_sp").Length;
        collectedKeys = 0;
    }

    private void Start()
    {
        AudioManager.instance.MarkAsMusic(Audio.M_AMBIENCE);
        AudioManager.instance.MarkAsMusic(Audio.M_VICTORY);

        AudioManager.instance.SetLooping(Audio.M_AMBIENCE, true);
        AudioManager.instance.Play(Audio.M_AMBIENCE);

        SetCursorVisible(false);
    }

    public void OnPlayerCollectKey()
    {
        if (++collectedKeys == totalKeys) finishPortal.TurnOn();
    }

    public IEnumerator DieAfter(float t)
    {
        gameOver = true;
        yield return new WaitForSeconds(t);
        SetCursorVisible(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void SetCursorVisible(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
