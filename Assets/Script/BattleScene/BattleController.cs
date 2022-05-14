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
    [SerializeField] EncycData encycData;
    SaveData data;
    int restNum;
    string rankStr;
    int rankInt;
    int encycCompletion;
    bool alreadyWin;

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

        // 味方0で敗北UI表示。敵0で勝利UI表示。両方0になった時、0になったスピードが早い方が負け。
        if (Homes.Length == 0 && !alreadyWin)
        {
            LoseUI.SetActive(true);
        }
        else if (Enemies.Length == 0)
        {
            WinUI.SetActive(true);
            alreadyWin = true;
        }
    }


    // Retryボタン。Retryすると連勝数加算されない。ここで前回セーブしたPreparationSceneをそのまま表示し直す。
    public void returnToPreparationScene()
    {
        GameObject.Find("DataForRetry").tag = "Retried";
        SceneManager.LoadScene("PreparationScene");
    }


    // Titleボタン。DontDestroyOnloadを解除。
    public void moveToTitleScene()
    {
        calculateRank();
        SceneManager.MoveGameObjectToScene(GameObject.Find("DataForRetry"), SceneManager.GetActiveScene()); 
        SceneManager.LoadScene("TitleScene");
    }


    // Nextボタン。DontDestroyOnloadを解除
    public void nextToPreparationScene()
    {
        calculateRank();
        SceneManager.MoveGameObjectToScene(GameObject.Find("DataForRetry"), SceneManager.GetActiveScene());
        SceneManager.LoadScene("PreparationScene");
    }


    
    void calculateRank()
    {
        // 連勝数＋１
        data.WinningStreak += 1;

        // 連勝数 - 各Difficultyのphase数 で現在ランクと次のランクまでの勝利数を計算。
        int winStForCal = data.WinningStreak;
        for (int i = 0; 0 <= winStForCal && i < difficultyData.DifficultyList.Count; i++)
        {
            winStForCal -= difficultyData.DifficultyList[i].PhaseNum;
            rankStr = difficultyData.DifficultyList[i].RankStr;
            rankInt = difficultyData.DifficultyList[i].RankInt;
            restNum = -winStForCal;

            // ランクが最大(S)になった時に「次ランクまでの勝利数」を常に０に。
            if (restNum < 0) restNum = 0;
        }

        // 現在ランクから図鑑完成率を計算。現在ランク以下の図鑑項数 ÷ 全項数。
        int completionNum = 0;
        for(int i = 0; i < encycData.EncycList.Count; i++)
        {
            if (encycData.EncycList[i].RankInt <= rankInt) completionNum += 1;
        }
        encycCompletion = completionNum * 100 / encycData.EncycList.Count;

        //  セーブデータに書き込み
        SaveData.SetSaveData(data.WinningStreak, rankStr, rankInt, restNum, encycCompletion);
    }
}
