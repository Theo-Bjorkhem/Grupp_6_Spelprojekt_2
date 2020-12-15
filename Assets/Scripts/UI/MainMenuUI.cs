using UnityEngine;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField]
    GameObject[] Canvases;

    public void OnStartButtonClicked()
    {
        if (AudioManager.ourInstance != null)
        {
            AudioManager.ourInstance.PlaySound("MenuPositive");
        }
        GameManager.ourInstance.TransitionToStage(0);
    }
    public void LoadScene(int aScene)
    {
        if (AudioManager.ourInstance != null)
        {
            AudioManager.ourInstance.PlaySound("MenuPositive");
        }
        GameManager.ourInstance.TransitionToStage(aScene);
    }

    public void LoadCanvas(int aCanvas)
    {
        if (AudioManager.ourInstance != null)
        {
            AudioManager.ourInstance.PlaySound("MenuPositive");
        }
        foreach (var item in Canvases)
        {
            item.SetActive(false);
        }
        Canvases[aCanvas].SetActive(true);

    }
    public void OnReturnButtonPressed()
    {
        if (AudioManager.ourInstance != null)
        {
            AudioManager.ourInstance.PlaySound("MenuNegative");
        }
        foreach (var item in Canvases)
        {
            item.SetActive(false);
        }
        Canvases[0].SetActive(true);
        Canvases[3].SetActive(true);
    }
}
