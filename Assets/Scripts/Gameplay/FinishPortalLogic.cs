using UnityEngine;

public class FinishPortalLogic : MonoBehaviour
{
    [SerializeField] private Material offMat, onMat;
    private new Renderer renderer;
    private GameObject psystem;
    private HoverEffect hover;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        psystem = transform.GetChild(0).gameObject;
        hover = GetComponent<HoverEffect>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        renderer.material = offMat;
        psystem.SetActive(false);
        hover.enabled = false;
    }

    // Update is called once per frame
    public void TurnOn()
    {
        renderer.material = onMat;
        psystem.SetActive(true);
        hover.enabled = true;
        LevelManager.instance.audioMng.Play("sfx_orb_active");
    }
}
