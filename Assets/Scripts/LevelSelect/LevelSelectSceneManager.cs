using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectSceneManager : MonoBehaviour
{
    public Button[] buttons;
    public int LevelCount;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.MarkAsMusic(Audio.LEVELSELECT);

        AudioManager.instance.SetLooping(Audio.LEVELSELECT, true);
        AudioManager.instance.SetVolume(Audio.LEVELSELECT, 0.5f);
        AudioManager.instance.Play(Audio.LEVELSELECT);
        AudioManager.instance.SetVolume(Audio.ORB_ACTIVE, 0.1f);

        PortalControl portal = FindObjectOfType<PortalControl>();
        portal.TurnOn();
        portal.GetComponent<AudioSource>().volume = 0.04f;


        for (int i=0; i<buttons.Length; i++)
        {
            TextMeshProUGUI text = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
            text.text = (i + 1).ToString();

            if (i >= LevelCount) buttons[i].interactable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
