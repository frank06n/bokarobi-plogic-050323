using UnityEngine;

public class FinishPortalLogic : MonoBehaviour
{
    [SerializeField] private Material offMat, onMat;
    private new Renderer renderer;
    private new GameObject particleSystem;
    private HoverEffect hoverEffect;
    private bool on;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        particleSystem = transform.GetChild(0).gameObject;
        hoverEffect = GetComponent<HoverEffect>();
    }

    private void Start()
    {
        SetState(false);
    }

    public void TurnOn()
    {
        SetState(true);
        AudioManager.instance.Play(Audio.ORB_ACTIVE);
    }

    private void SetState(bool on)
    {
        this.on = on;
        renderer.material = on ? onMat : offMat;
        particleSystem.SetActive(on);
        hoverEffect.enabled = on;
    }

    public bool isOn()
    {
        return on;
    }
}
