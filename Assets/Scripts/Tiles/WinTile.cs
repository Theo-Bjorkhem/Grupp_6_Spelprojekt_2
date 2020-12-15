using UnityEngine;
using UnityEngine.SceneManagement;

public class WinTile : Tile
{
    protected override void Start()
    {
        base.Start();


        StageManager.ourInstance.RegisterTileForTurnEvents(this);
    }

    public override void OnEnter(Entity steppedOnMe)
    {
        if (steppedOnMe is Player)
        {
           StageManager.ourInstance.OnPlayerWon();
           // 
           // if (SceneManager.GetActiveScene().buildIndex == 11)
           // {
           //     Debug.Log("Level 11");
           // }
           // else
           // {
           //     SceneManager.LoadScene(nextSceneLoad);
           //
           //     if (nextSceneLoad > PlayerPrefs.GetInt("levelAt"))
           //     {
           //         PlayerPrefs.SetInt("levelAt", nextSceneLoad);
           //     }
           // }
        }
        base.OnEnter(steppedOnMe);
    }
}
