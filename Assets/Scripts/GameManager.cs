using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    [SerializeField] PlayerInput inputActions;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject playerCameraPrefab;
    [SerializeField] Transform playerSpawnTransform;
    [SerializeField] AudioMixer audioMixer;

    public static GameManager Instance { get; private set; }

    public PlaneController playerPlaneController { get; private set; }

    public delegate void GameOverHandler();
    public static event GameOverHandler OnGameOver;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
        SpawnPlayer();
        audioMixer.SetFloat("InGameEffectsVolume", 0f);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void SpawnPlayer()
    {
        GameObject player = Instantiate(playerPrefab, playerSpawnTransform.position, playerSpawnTransform.rotation);
        GameObject playerCamera = Instantiate(playerCameraPrefab, playerSpawnTransform.position, playerSpawnTransform.rotation);
        playerPlaneController = player.GetComponent<PlaneController>();
        playerCamera.GetComponent<CinemachineCamera>().Follow = player.transform;
        playerCamera.GetComponent<CinemachineCamera>().LookAt = player.transform;
        CinemachineInputAxisController cameraAxisController = playerCamera.GetComponent<CinemachineInputAxisController>();
        playerPlaneController.Initialize(inputActions, cameraAxisController);
        playerPlaneController.OnPlaneCrash += PlaneCrash;
        SetActionMap("Game");
    }

    void PlaneCrash()
    {
        Invoke(nameof(GameOver), 2f);
    }

    void GameOver()
    {
        audioMixer.SetFloat("InGameEffectsVolume", -144.0f);
        Time.timeScale = 0;
        OnGameOver?.Invoke();
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetActionMap(string actionMap)
    {
        inputActions.actions.Disable();
        inputActions.actions.FindActionMap(actionMap).Enable();
    }

    public PlayerInput GetPlayerInputReference()
    {
        return inputActions;
    }
}
