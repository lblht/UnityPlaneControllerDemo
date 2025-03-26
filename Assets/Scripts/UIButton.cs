using UnityEngine;
using UnityEngine.Events;

public class UIButton : MonoBehaviour
{
    public UnityEvent onSubmitAction;
    [SerializeField] AudioSource selectAudioSource;
    [SerializeField] AudioSource submitAudioSource;

    public void PlaySelectSound()
    {
        selectAudioSource.Play();
    }

    public void PlaySubmitSound()
    {
        submitAudioSource.Play();
    }

    public void ButtonPressed()
    {
        Time.timeScale = 1;
        Invoke(nameof(InvokeButtonAction), 0.5f);
    }

    void InvokeButtonAction()
    {
        onSubmitAction?.Invoke();
    }
}
