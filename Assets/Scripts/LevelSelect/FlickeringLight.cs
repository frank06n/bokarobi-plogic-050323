using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    private Light streetLight;
    public float steadyDurationMin = 2f; // Minimum duration for the steady state
    public float steadyDurationMax = 5f; // Maximum duration for the steady state
    public float flickerDurationMin = 0.1f; // Minimum duration for the flickering state
    public float flickerDurationMax = 0.5f; // Maximum duration for the flickering state
    public float flickerIntervalMin = 0.01f; // Minimum interval between flicker changes
    public float flickerIntervalMax = 0.1f; // Maximum interval between flicker changes
    public float minIntensity = 0.5f; // Minimum intensity of the flicker
    public float maxIntensity = 1.5f; // Maximum intensity of the flicker

    private enum State
    {
        Steady,
        Flickering
    }

    private State currentState;
    private float stateDuration;
    private float timeElapsed;

    void Start()
    {
        streetLight = GetComponent<Light>();

        SetRandomStateDuration();
        currentState = State.Steady;
    }

    void Update()
    {
        if (streetLight == null)
            return;

        timeElapsed += Time.deltaTime;

        if (timeElapsed >= stateDuration)
        {
            SwitchState();
            timeElapsed = 0f;
        }

        if (currentState == State.Flickering)
        {
            FlickerLight();
        }
    }

    void SwitchState()
    {
        if (currentState == State.Steady)
        {
            currentState = State.Flickering;
            stateDuration = Random.Range(flickerDurationMin, flickerDurationMax);
        }
        else
        {
            currentState = State.Steady;
            stateDuration = Random.Range(steadyDurationMin, steadyDurationMax);
        }
    }

    void FlickerLight()
    {
        float flickerInterval = Random.Range(flickerIntervalMin, flickerIntervalMax);
        if (timeElapsed % flickerInterval < flickerInterval / 2)
        {
            streetLight.intensity = Random.Range(minIntensity, maxIntensity);
        }
        else
        {
            streetLight.intensity = maxIntensity;
        }
    }

    void SetRandomStateDuration()
    {
        stateDuration = Random.Range(steadyDurationMin, steadyDurationMax);
    }
}
