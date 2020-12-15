using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeletePlayerPrefs : MonoBehaviour
{
    private void resetPlayerPrefs()
    {
        //lvlButtons.interactable = false;
        PlayerPrefs.DeleteAll();
    }
}
