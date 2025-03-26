using UnityEngine;
using UnityEngine.UI;

public class PlaneUIManager : MonoBehaviour
{
    [SerializeField] PlaneController planeController;
    [SerializeField] GameObject planeUICanvas;
    [SerializeField] Image throttleHandleTop;
    [SerializeField] Image throttleHandleBottom;
    [SerializeField] Image engineSwitchImage;
    [SerializeField] Image fuelArrow;

    void OnEnable()
    {
        planeController.OnAccelerationChanged += UpdateThrottleHandle;
        planeController.OnEngineSwitch += UpdateEngineSwitch;
        planeController.OnFuelChange += UpdateFuelArrow;
    }

    void OnDisable()
    {
        planeController.OnAccelerationChanged -= UpdateThrottleHandle;
        planeController.OnEngineSwitch -= UpdateEngineSwitch;
        planeController.OnFuelChange -= UpdateFuelArrow;
    }

    void UpdateEngineSwitch(bool engine)
    {
        float rotation = engine ? 0f : 180f;
        engineSwitchImage.rectTransform.rotation = Quaternion.Euler(0, 0, rotation);
    }

    void UpdateThrottleHandle(float acceleration, float maxAcceleration)
    {

        float handleYPosition = Mathf.Lerp(-66f, 66f, acceleration / maxAcceleration);
        throttleHandleTop.rectTransform.localPosition = new Vector3(throttleHandleTop.rectTransform.localPosition.x, handleYPosition, throttleHandleTop.rectTransform.localPosition.y);

        float handleHeight;
        float handleRotation;
        if (acceleration < maxAcceleration / 2)
        {
            handleRotation = 180f;
            handleHeight = Mathf.Lerp(66f, 0, acceleration / (maxAcceleration / 2));
        }
        else
        {
            handleRotation = 0f;
            handleHeight = Mathf.Lerp(0, 66f, (acceleration - (maxAcceleration / 2)) / (maxAcceleration / 2));
        }
        throttleHandleBottom.rectTransform.rotation = Quaternion.Euler(0, 0, handleRotation);
        throttleHandleBottom.rectTransform.sizeDelta = new Vector2(throttleHandleBottom.rectTransform.sizeDelta.x, handleHeight);
    }

    void UpdateFuelArrow(float fuel, float maxFuel)
    {
        float rotation = Mathf.Lerp(-142f, 142f, 1 - (fuel / maxFuel));
        fuelArrow.rectTransform.localRotation = Quaternion.Euler(0, 0, rotation);
    }
}
