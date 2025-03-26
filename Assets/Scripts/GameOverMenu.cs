using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    [SerializeField] Button restartButton;

    void OnEnable()
    {
        GameManager.OnGameOver += EnableGameOverScreen;
    }

    void OnDisable()
    {
        GameManager.OnGameOver -= EnableGameOverScreen;
    }

    void EnableGameOverScreen()
    {
        canvas.SetActive(true);
        GameManager.Instance.SetActionMap("UI");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        restartButton.Select();
    }

    public void RestartGame()
    {
        GameManager.Instance.LoadScene("World");
    }

    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }
}
