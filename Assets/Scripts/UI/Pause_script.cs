using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause_script : MonoBehaviour
{
    public void PauseGame()
    {
        gameObject.SetActive(true);
    }
    public void UnPasueGame()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        
    }
}
