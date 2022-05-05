using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ScriptExcutionOrderで一番初めに呼ばれるように設定。
public class BattleController : MonoBehaviour
{

    public GameObject[] Enemies;
    public GameObject[] Homes;
    [SerializeField] GameObject LoseUI;
    [SerializeField] GameObject WinUI;


    private void Start()
    {
        WinUI.SetActive(false);
        LoseUI.SetActive(false);
    }


    // これのメモリ消費えぐい。キャラがdestroyされるたびに呼び出したかったがうまくいかなかった。
    void Update()
    {
        Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Homes = GameObject.FindGameObjectsWithTag("Home");

        // 味方0で敗北UI表示。敵0で勝利UI表示。両方0だと敗北。
        if (Homes.Length == 0)
        {
            LoseUI.SetActive(true);
        }
        else if (Enemies.Length == 0)
        {
            WinUI.SetActive(true);
        }
    }


    public void moveToTitleScene()
    {
        SceneManager.LoadScene("TitleScene");
    }


    // ここで前回セーブしたPreparationSceneをそのまま表示し直す。
    public void returnToPreparationScene()
    {
        SceneManager.LoadScene("PreparationScene");
    }


}
