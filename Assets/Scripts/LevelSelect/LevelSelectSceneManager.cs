using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelSelectSceneManager : MonoBehaviour
{
    public static LevelSelectSceneManager instance;
    public Button[] buttons;
    public int LevelCount;

    public CanvasGroup MainmenuPanel, LevelsPanel;
    [HideInInspector] public static bool ShouldShowLevels = false;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.MarkAsMusic(Audio.LEVELSELECT);

        AudioManager.instance.SetLooping(Audio.LEVELSELECT, true);
        AudioManager.instance.SetVolume(Audio.LEVELSELECT, 0.5f);
        AudioManager.instance.Play(Audio.LEVELSELECT);
        AudioManager.instance.SetVolume(Audio.PORTAL_ACTIVE, 0.1f);

        PortalControl portal = FindFirstObjectByType<PortalControl>();
        portal.TurnOn();
        portal.GetComponent<AudioSource>().volume = 0.04f;


        for (int i=0; i<buttons.Length; i++)
        {
            int level = i + 1;
            TextMeshProUGUI text = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
            text.text = level.ToString();

            if (i >= LevelCount) buttons[i].interactable = false;
            buttons[i].onClick.AddListener(() => SceneManager.LoadScene(level));
        }

        // set active, if was set inactive during development
        MainmenuPanel.gameObject.SetActive(true);
        LevelsPanel.gameObject.SetActive(true);

        if (LevelSelectSceneManager.ShouldShowLevels)
        {
            LevelSelectSceneManager.ShouldShowLevels = false;
            SetPanelActive(LevelsPanel, true);
            SetPanelActive(MainmenuPanel, false);
        }
        else
        {
            SetPanelActive(MainmenuPanel, true);
            SetPanelActive(LevelsPanel, false);
        }
    }

    private void SetPanelActive(CanvasGroup panel, bool active)
    {
        panel.alpha = active ? 1 : 0;
        panel.interactable = active;
        panel.blocksRaycasts = active;
    }

    public void ShowLevels()
    {
        StartCoroutine(SwitchPanels(MainmenuPanel, LevelsPanel));
    }
    public void ShowMainMenu()
    {
        StartCoroutine(SwitchPanels(LevelsPanel, MainmenuPanel));
    }

    IEnumerator SwitchPanels(CanvasGroup toHide, CanvasGroup toShow)
    {
        const float SwitchDuration = 0.8f;
        const float HalfSD = SwitchDuration / 2f;
        float elapsedTime = 0f;

        while (elapsedTime < HalfSD)
        {
            toHide.alpha = 1 - (elapsedTime / HalfSD);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetPanelActive(toHide, false);

        while (elapsedTime < SwitchDuration)
        {
            toShow.alpha = (elapsedTime - HalfSD) / HalfSD;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetPanelActive(toShow, true);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
