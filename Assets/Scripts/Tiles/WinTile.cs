using UnityEngine;
using UnityEngine.SceneManagement;

public class WinTile : Tile
{
    public int nextSceneLoad;
    protected override void Start()
    {
        base.Start();

        nextSceneLoad = SceneManager.GetActiveScene().buildIndex - 3;

        StageManager.ourInstance.RegisterTileForTurnEvents(this);
    }

    public override void OnEnter(Entity steppedOnMe)
    {
        if (steppedOnMe is Player)
        {
            //if (SceneManager.GetActiveScene().buildIndex)
            //{
            //    Debug.Log("Hello");
            //    //SceneManager.GetActiveScene().buildIndex = 0
            //    //SceneManager.LoadScene("mainMenu_scene");
            //}
            StageManager.ourInstance.OnPlayerWon();

            if (SceneManager.GetActiveScene().buildIndex == 7)
            {
                Debug.Log("Level 11");
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
