using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleController : MonoBehaviour
{
    [SerializeField] GameObject saveData;

    void Start()
    {
        
    }

    public void newGame()
    {
        saveData.SetActive(true);
    }

    public void loadGame()
    {
        saveData.SetActive(true);
    }

    public void backToTitle()
    {
        saveData.SetActive(false);
    }


    public void encyclopedia()
    {

    }
}
