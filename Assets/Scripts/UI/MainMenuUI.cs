using UnityEngine;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField]
    GameObject[] Canvases;

    public void OnStartButtonClicked()
    {
        GameManager.ourInstance.TransitionToStage(0);
    }
    public void LoadScene(int aScene)
    {
        GameManager.ourInstance.TransitionToStage(aScene);
    }

    public void LoadCanvas(int aCanvas)
    {
        foreach (var item in Canvases)
        {
            item.SetActive(false);
        }
        Canvases[aCanvas].SetActive(true);

    }
    public void OnReturnButtonPressed()
    {
        foreach (var item in Canvases)
        {
            item.SetActive(false);
        }
        Canvases[0].SetActive(true);
    }
}
