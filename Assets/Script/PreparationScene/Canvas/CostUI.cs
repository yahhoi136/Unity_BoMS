using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CostUI : MonoBehaviour
{
    [SerializeField] PlayerPreparation playerpreparation;
    [SerializeField] Text thisText;
    [SerializeField] Text costOverText;
    [SerializeField] Button startButton;

    Color costOver = new Color(1.0f, 0f, 0f);
    Color withinCost = new Color(0.2f, 0.2f, 1.0f);


    // Cost, CostOverの表示と、CostOver時にstartButtonを無効化。
    void Update()
    {
        thisText.text = $"{playerpreparation.TotalHomeCost} / {playerpreparation.HomeCostLimit}";

        if(playerpreparation.HomeCostLimit < playerpreparation.TotalHomeCost)
        {
            thisText.color = costOver;
            costOverText.enabled = true;
            startButton.interactable = false;
        }
        else
        {
            thisText.color = withinCost;
            costOverText.enabled = false;
            startButton.interactable = true;
        }
    }
}
