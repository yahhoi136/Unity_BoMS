using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

// ScriptExcutionOrderで一番初めに呼ばれるように設定。
public class BattleController : MonoBehaviour
{

    public GameObject[] Enemies;
    public GameObject[] Homes;
    [SerializeField] GameObject LoseUI;
    [SerializeField] GameObject WinUI;

    [SerializeField] DifficultyData difficultyData;
    SaveData data;
    int restNum;
    string rank;
    bool onceDone;
    bool dieAfterWin;

    private void Start()
    {
        try
        {
            // セーブデータ読み込み
            using (var reader = new StreamReader(Application.persistentDataPath + "/SaveData.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                data = (SaveData)serializer.Deserialize(reader, typeof(SaveData));
            }
        }
        // ファイルがない場合
        catch (FileNotFoundException)
        {
            print("ファイル無いよ");
            print(Application.persistentDataPath);
        }

    }


    // これのメモリ消費えぐい。キャラがdestroyされるたびに呼び出したかったけどうまくいかなかった。
    void Update()
    {
        Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Homes = GameObject.FindGameObjectsWithTag("Home");

        // 味方0で敗北UI表示。敵0で勝利UI表示。両方0になった時、0になったスピード差で勝敗決定。
        if (Homes.Length == 0 && !dieAfterWin)
        {
            LoseUI.SetActive(true);
        }
        else if (Enemies.Length == 0 && onceDone == false)
        {
            WinUI.SetActive(true);
            dieAfterWin = true;
        }
    }


    // 連勝数 - 各Difficultyのphase数 でランクを計算。
    void calculateRank()
    {
        // 連勝数＋１
        data.WinningStreak += 1;
        int winStForCal = data.WinningStreak;

        for (int i = 0; 0 <= winStForCal && i < difficultyData.DifficultyList.Count; i++)
        {
            winStForCal -= difficultyData.DifficultyList[i].NumOfPhase;
            rank = difficultyData.DifficultyList[i].Rank;
            restNum = -winStForCal;

            // ランクが最大(S)になった時に「次ランクまでの勝利数」を常に０に。
            if (restNum < 0) restNum = 0;
        }

        SaveData.SetSaveData(data.WinningStreak, rank, restNum, data.Completion);
        onceDone = true;
    }


    // Retry。Retryすると連勝数加算されない。ここで前回セーブしたPreparationSceneをそのまま表示し直す。
    public void returnToPreparationScene()
    {
        GameObject.Find("DataForRetry").tag = "Retried";
        SceneManager.LoadScene("PreparationScene");
    }


    // Title。DontDestroyOnloadを解除。
    public void moveToTitleScene()
    {
        calculateRank();
        SceneManager.MoveGameObjectToScene(GameObject.Find("DataForRetry"), SceneManager.GetActiveScene()); 
        SceneManager.LoadScene("TitleScene");
    }


    // Next。DontDestroyOnloadを解除
    public void nextToPreparationScene()
    {
        calculateRank();
        SceneManager.MoveGameObjectToScene(GameObject.Find("DataForRetry"), SceneManager.GetActiveScene());
        SceneManager.LoadScene("PreparationScene");
    }

}
