﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class WinTile : Tile
{
    public int nextSceneLoad;
    protected override void Start()
    {
        base.Start();

        nextSceneLoad = SceneManager.GetActiveScene().buildIndex + 1;

        StageManager.ourInstance.RegisterTileForTurnEvents(this);
    }

    public override void OnEnter(Entity steppedOnMe)
    {
        if (steppedOnMe is Player)
        {
            //LevelControlScript.instance.youWin();
            StageManager.ourInstance.OnPlayerWon();
            
            Debug.Log("You hit!");
            if (SceneManager.GetActiveScene().buildIndex == 7)
            {
                Debug.Log("You Completed ALL Levels");
            }
            else
            {
                SceneManager.LoadScene(nextSceneLoad);

                if (nextSceneLoad > PlayerPrefs.GetInt("levelAt"))
                {
                    PlayerPrefs.SetInt("levelAt", nextSceneLoad);
                }
            }
        }
        base.OnEnter(steppedOnMe);
    }
}
