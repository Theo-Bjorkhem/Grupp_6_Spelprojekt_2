using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnCounterUI : MonoBehaviour
{
    private int myTurnsLeft;
    private void Start()
    {
        myTurnsLeft = StageManager.ourInstance.myTurnsLeft;
        StageManager.ourInstance.myStageMessages.myOnTurnStart += OnTurnStart;
        SetTurnCounterUI();
    }
    private void SetTurnCounterUI()
    {
        gameObject.GetComponent<Text>().text = "" + myTurnsLeft;
    }
    private void OnTurnStart()
    {
        myTurnsLeft--;
        SetTurnCounterUI();
    }
}
