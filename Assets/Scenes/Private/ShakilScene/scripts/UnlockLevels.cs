using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UnlockLevels : MonoBehaviour
{
    public Button lv2, lv3, lv4, lv5, lv6;
    int levelPassed;

    // Start is called before the first frame update
    void Start()
    {
        levelPassed = PlayerPrefs.GetInt("LevelPassed");
        lv2.interactable = false;
        lv3.interactable = false;
        lv4.interactable = false;
        lv5.interactable = false;
        lv6.interactable = false;

        switch (levelPassed)
        {
            case 1:
                lv2.interactable = true;
                break;
            case 2:
                lv2.interactable = true;
                lv3.interactable = true;
                break;
            case 3:
                lv2.interactable = true;
                lv3.interactable = true;
                lv4.interactable = true;
                break;
            case 4:
                lv2.interactable = true;
                lv3.interactable = true;
                lv4.interactable = true;
                lv5.interactable = true;
                break;
            case 5:
                lv2.interactable = true;
                lv3.interactable = true;
                lv4.interactable = true;
                lv5.interactable = true;
                lv6.interactable = true;
                break;
        }
    }

    public void LevelToLoad (int level)
    {
        SceneManager.LoadScene(level);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
