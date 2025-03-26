using UnityEngine;

public class PlaneAudioManager : MonoBehaviour
{
    [SerializeField] PlaneController planeController;
    [SerializeField] AudioSource engineSound;
    [SerializeField] AudioSource engineStartSound;
    [SerializeField] AudioSource engineStopSound;
    [SerializeField] AudioSource engineSwitchSound;

    void OnEnable()
    {
        planeController.OnAccelerationChanged += UpdateEnginePitch;
        planeController.OnEngineSwitch += EngineSwitch;
    }

    void OnDisable()
    {
        planeController.OnAccelerationChanged -= UpdateEnginePitch;
        planeController.OnEngineSwitch -= EngineSwitch;
    }

    public void EngineSwitch(bool value)
    {
        if (value)
        {
            engineSwitchSound.Play();
            engineStartSound.Play();
            engineSound.Play();
        }
        else
        {
            if (engineSound.isPlaying)
            {
                engineStopSound.Play();
                engineSound.Stop();
                engineSwitchSound.Play();
            }
        }
    }

    void UpdateEnginePitch(float acceleration, float maxAcceleration)
    {
        float accelerationDifference = acceleration / maxAcceleration;
        float pitch = Mathf.Lerp(0.5f, 1f, accelerationDifference);
        engineSound.pitch = pitch;
    }
}
