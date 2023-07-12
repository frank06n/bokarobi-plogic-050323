using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public float DeathY;
    public float Gravity;
    public LayerMask Ground;

    [HideInInspector] public bool gameOver;

    private int collectedKeys;
    private int totalKeys;
    private FinishPortalLogic finishPortal;
    private bool isPortalOn;


    [HideInInspector] public AudioManager audioMng;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;

        audioMng = FindObjectOfType<AudioManager>();
        finishPortal = FindObjectOfType<FinishPortalLogic>();

        gameOver = false;

        totalKeys = GameObject.FindGameObjectsWithTag("pickup_sp").Length;
        collectedKeys = 0;
    }

    private void Start()
    {
        // Play BGM
        audioMng.Play("sfx_ambience");

        // Lock and Hide the Cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ObtainedKey()
    {
        //Debug.Log(collectedKeys+1 + " :: " + totalKeys);
        if (++collectedKeys == totalKeys)
        {
            finishPortal.TurnOn();
            isPortalOn = true;
        }
    }

    public bool IsPortalOn()
    {
        return isPortalOn;
    }


    public IEnumerator DieAfter(float t)
    {
        gameOver = true;
        yield return new WaitForSeconds(t);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
