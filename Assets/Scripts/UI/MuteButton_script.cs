using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteButton_script : MonoBehaviour
{
    [SerializeField] Image myOnSymbol;
    [SerializeField] Image myOffSymbol;
    private void Start()
    {
        if (AudioManager.ourInstance.IsMuted())
        {
            this.GetComponent<Image>().sprite = myOffSymbol.sprite;
        }
        else
        {
            this.GetComponent<Image>().sprite = myOnSymbol.sprite;
        }
    }
    public void MuteButton()
    {
        AudioManager.ourInstance.ToggleMute();
        if (this.GetComponent<Image>().sprite == myOffSymbol.sprite)
        {
            this.GetComponent<Image>().sprite = myOnSymbol.sprite;
        }
        else
        {
            this.GetComponent<Image>().sprite = myOffSymbol.sprite;
        }
    }
}
