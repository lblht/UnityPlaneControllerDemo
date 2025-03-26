using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaneController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Rigidbody rb;
    [Header("Properties")]
    [SerializeField] float maxAcceleration;
    [SerializeField] float maxSpeed;
    [SerializeField] float yawAmount;
    [SerializeField] float pitchAmount;
    [SerializeField] float rollAmount;
    [SerializeField] bool engine;
    [SerializeField] float throttleIncreaseRate;
    [SerializeField] float slowDampening;
    [SerializeField] float gravityCencelationLimit;
    [SerializeField] Vector2 heightLimit;
    [SerializeField] float maxFuel;
    [SerializeField] float fuelDepletionRate;

    public delegate void AccelerationChangeHandler(float acceleration, float maxAcceleration);
    public event AccelerationChangeHandler OnAccelerationChanged;

    public delegate void EnginSwitchHandler(bool value);
    public event EnginSwitchHandler OnEngineSwitch;

    public delegate void VelocityChangeHandler(Vector3 velocity);
    public event VelocityChangeHandler OnVelocityChange;

    public delegate void FuelChangeHandler(float fuel, float maxFuel);
    public event FuelChangeHandler OnFuelChange;

    public delegate void PlaneCrashHandler();
    public event PlaneCrashHandler OnPlaneCrash;

    PlayerInput inputActions;
    CinemachineInputAxisController cameraController;

    float horizontalInput;
    float verticalInput;
    float rollInput;
    float accelerationChangeInput;

    float acceleration;
    float yaw;
    float pitch;
    float roll;

    float fastDampening;

    float fuel;

    void SetupInputEvents()
    {
        if (inputActions != null)
        {
            inputActions.actions["Engine"].started += ToggleEngine;
            inputActions.actions["Camera"].started += ctx => cameraController.enabled = true;
            inputActions.actions["Camera"].canceled += ctx => cameraController.enabled = false;
        }
    }

    void OnDisable()
    {
        if (inputActions != null)
        {
            inputActions.actions["Engine"].started -= ToggleEngine;
            inputActions.actions["Camera"].started -= ctx => cameraController.enabled = true;
            inputActions.actions["Camera"].canceled -= ctx => cameraController.enabled = false;
        }
    }

    void Start()
    {
        cameraController.enabled = false;
        fastDampening = maxAcceleration / maxSpeed;
        fuel = maxFuel;
        OnFuelChange?.Invoke(fuel, maxFuel);
        SetEngine(engine);
    }

    public void Initialize(PlayerInput inputActions, CinemachineInputAxisController cameraController)
    {
        this.inputActions = inputActions;
        this.cameraController = cameraController;
        SetupInputEvents();
    }

    void Update()
    {
        GetInput();
    }

    void FixedUpdate()
    {
        if (fuel <= 0) { SetEngine(false); }
        if (transform.position.y > heightLimit.y) { SetEngine(false); }
        if (transform.position.y < heightLimit.x)
        {
            SetEngine(false);
            cameraController.gameObject.SetActive(false);
            OnPlaneCrash?.Invoke();
            this.enabled = false;
        }
        if (engine)
        {
            ApplyForwardMovement();
            ChangeAcceleration();
            if (acceleration > 0) { UpdateFuel(); }
        }
        ApplyPitchRollYaw();
        ApplyGravity();
        ApplyDynamicDampening();
        OnVelocityChange?.Invoke(rb.linearVelocity);

        if (Mathf.Abs(transform.position.x) > 5000f || Mathf.Abs(transform.position.z) > 5000f)
        {
            transform.position = new Vector3(-transform.position.x, transform.position.y, -transform.position.z);
        }
    }

    void GetInput()
    {
        horizontalInput = inputActions.actions["PitchYaw"].ReadValue<Vector2>().x;
        verticalInput = inputActions.actions["PitchYaw"].ReadValue<Vector2>().y;
        rollInput = inputActions.actions["Roll"].ReadValue<float>();
        accelerationChangeInput = inputActions.actions["AccelerationChange"].ReadValue<float>();
    }

    void ApplyForwardMovement()
    {
        rb.AddForce(transform.forward * acceleration);
    }

    void ApplyPitchRollYaw()
    {
        yaw = horizontalInput * yawAmount;
        pitch = verticalInput * pitchAmount;
        roll = -rollInput * rollAmount;

        if (cameraController.enabled) { roll = 0f; }

        float controlMultiplier = Vector3.ClampMagnitude(rb.linearVelocity, 10).magnitude;
        rb.AddTorque(transform.up * yaw * controlMultiplier);
        rb.AddTorque(transform.right * pitch * controlMultiplier);
        rb.AddTorque(transform.forward * roll * controlMultiplier);
    }

    void ApplyDynamicDampening()
    {
        float speedDifference = acceleration / maxAcceleration;
        float linearDamping;
        linearDamping = Mathf.Lerp(slowDampening, fastDampening, speedDifference);
        rb.linearDamping = linearDamping;
    }

    void ApplyGravity()
    {
        Vector3 velocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float gravityScale = 1 - Mathf.Clamp((velocity.magnitude + acceleration) / (gravityCencelationLimit * 2f), 0, 1);
        Vector3 gravityForce = 9.81f * gravityScale * Vector3.down;
        rb.AddForce(gravityForce, ForceMode.Acceleration);
    }

    void ToggleEngine(InputAction.CallbackContext context)
    {
        SetEngine(!engine);
    }

    public void SetEngine(bool value)
    {
        engine = value;
        acceleration = 0;
        OnEngineSwitch?.Invoke(engine);
        OnAccelerationChanged?.Invoke(acceleration, maxAcceleration);
    }

    void ChangeAcceleration()
    {
        acceleration = Mathf.Clamp(acceleration + (throttleIncreaseRate * accelerationChangeInput), 0, maxAcceleration);
        OnAccelerationChanged?.Invoke(acceleration, maxAcceleration);
    }

    void UpdateFuel()
    {
        float accelerationModifier = Mathf.Clamp(acceleration / maxAcceleration, 0.5f, 1);
        float depletionAmount = fuelDepletionRate * accelerationModifier;
        fuel -= depletionAmount;
        fuel = Mathf.Clamp(fuel, 0, maxFuel);
        OnFuelChange?.Invoke(fuel, maxFuel);
    }
}
