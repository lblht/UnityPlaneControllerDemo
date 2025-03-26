using UnityEngine;

public class PlaneCosmeticsManager : MonoBehaviour
{
    [SerializeField] PlaneController planeController;
    [SerializeField] GameObject rotor;
    [SerializeField] GameObject blurEffect;
    [SerializeField] TrailRenderer[] trails;
    float rotorSpeed = 0f;
    bool engine;

    void OnEnable()
    {
        planeController.OnAccelerationChanged += UpdateRotor;
        planeController.OnAccelerationChanged += UpdateTrails;
        planeController.OnEngineSwitch += UpdateEngineValue;
    }

    void OnDisable()
    {
        planeController.OnAccelerationChanged -= UpdateRotor;
        planeController.OnAccelerationChanged -= UpdateTrails;
        planeController.OnEngineSwitch -= UpdateEngineValue;
    }

    void FixedUpdate()
    {
        if (!engine && rotorSpeed > 0) { rotorSpeed -= 0.1f; }
        else if (!engine) { rotorSpeed = 0; }
        rotor.transform.Rotate(0, 0, rotorSpeed);
    }

    void UpdateRotor(float acceleration, float maxAcceleration)
    {
        if (engine && acceleration > 0)
        {
            blurEffect.SetActive(true);
            float accelerationDifference = acceleration / maxAcceleration;
            rotorSpeed = Mathf.Lerp(20, 35, accelerationDifference);
        }
        else if (engine) { rotorSpeed = 10f; }
        else { blurEffect.SetActive(false); }

    }

    void UpdateTrails(float acceleration, float maxAcceleration)
    {
        bool emitting = acceleration > 0 ? true : false;
        foreach (TrailRenderer trail in trails) { trail.emitting = emitting; }
    }

    void UpdateEngineValue(bool engine)
    {
        this.engine = engine;
    }
}
