using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public static class Interactable
{
    public static readonly string Cactus = "cactus";
    public static readonly string WinOrb = "win";
    public static readonly string Pickup_Key = "pickup_sp";
    public static readonly string Pickup_DoubleJump = "pickup_dj";
    public static readonly string DeathY = "death_y";
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public float gravity;
    public LayerMask ground;

    [HideInInspector] public bool gameOver;
    [HideInInspector] public List<ObstacleController> obstacles;

    private int collectedKeys;
    private int totalKeys;
    [HideInInspector] public PortalControl finishPortal;

    [SerializeField] private TextMeshProUGUI hud_Keys;
    [SerializeField] private GameObject hudCrosshair;
    [SerializeField] private PopUpPanelController popUpPanelController;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;

        finishPortal = FindObjectOfType<PortalControl>();

        gameOver = false;

        totalKeys = GameObject.FindGameObjectsWithTag("pickup_sp").Length;
        collectedKeys = 0;

        obstacles = new List<ObstacleController>();
    }

    private void Start()
    {
        AudioManager.instance.MarkAsMusic(Audio.M_AMBIENCE);
        AudioManager.instance.MarkAsMusic(Audio.M_VICTORY);

        AudioManager.instance.SetLooping(Audio.M_AMBIENCE, true);
        AudioManager.instance.SetVolume(Audio.M_AMBIENCE,0.3f);
        AudioManager.instance.Play(Audio.M_AMBIENCE);

        SetCursorVisible(false);
        UpdateHUD_Keys();
    }

    public void OnPlayerCollectKey()
    {
        collectedKeys += 1;
        UpdateHUD_Keys();

        if (collectedKeys == totalKeys)
        {
            finishPortal.TurnOn();
            AudioManager.instance.Play(Audio.ORB_ACTIVE);
            AudioManager.instance.Stop(Audio.M_AMBIENCE);
            foreach (ObstacleController obst in obstacles) {
                obst.SetDead();
            }
        }
    }

    void UpdateHUD_Keys()
    {
        hud_Keys.text = $"{collectedKeys:D2} / {totalKeys:D2}";
    }

    public void Btn_Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetGameOver(bool win)
    {
        gameOver = true;
        hudCrosshair.SetActive(false);
        SetCursorVisible(true);
        popUpPanelController.ShowPanel(win ? "Level Clear!" : "You Died!");
    }

    private void SetCursorVisible(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
