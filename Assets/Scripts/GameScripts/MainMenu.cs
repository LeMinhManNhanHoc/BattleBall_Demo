using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioClip clickEffect;
    public AudioClip cheerEffect;
    public AudioClip passEffect;

    public AudioSource source;

    public void OnQuitButtonClicked()
    {
        source.PlayOneShot(clickEffect);
        Application.Quit();
    }

    public void OnLocalPVPClicked()
    {
        source.PlayOneShot(clickEffect);
        GameManager.Instance.isLocalPVP = true;
        SceneManager.LoadScene(1);
    }

    public void OnVSCPUClickeed()
    {
        source.PlayOneShot(clickEffect);
        source.Stop();
        GameManager.Instance.isLocalPVP = false;
        SceneManager.LoadScene(1);
    }
}
