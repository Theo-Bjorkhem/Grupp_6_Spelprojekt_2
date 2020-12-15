using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Not useable
/// </summary>
public class UnlockLevels : MonoBehaviour
{
    public Button lv2, lv3;
    int levelPassed;

    // Start is called before the first frame update
    void Start()
    {
        levelPassed = PlayerPrefs.GetInt("LevelPassed");
        lv2.interactable = false;
        lv3.interactable = false;

        //if (levelPassed)
        //{
        //    lv2.interactable = true;
        //}

        switch (levelPassed)
        {
            case 1:
                lv2.interactable = true;
                break;
            case 2:
                lv2.interactable = true;
                lv3.interactable = true;
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
