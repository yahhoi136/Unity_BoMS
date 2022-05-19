using System.Collections;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ScriptExcutionOrderでBattleSceneで一番初めに呼ばれるように設定。
public class BattleController : MonoBehaviour
{

    public GameObject[] Enemies;
    public GameObject[] Homes;
    [SerializeField] DifficultyData difficultyData;
    [SerializeField] EncycData encycData;
    [SerializeField] GameObject loseUI;
    [SerializeField] GameObject winUI;
    [SerializeField] GameObject releaseUI;
    [SerializeField] Text releaseText;
    [SerializeField] Text maxRank;
    SaveData data;
    int restNum;
    string rankStr;
    int rankInt;
    int encycCompletion;
    bool alreadyWin;
    bool rankUp;


    private void Start()
    {

        // セーブデータ読み込み
        using (var reader = new StreamReader(Application.persistentDataPath + "/SaveData.json"))
        {
            JsonSerializer serializer = new JsonSerializer();
            data = (SaveData)serializer.Deserialize(reader, typeof(SaveData));
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
            loseUI.SetActive(true);
        }
        else if (Enemies.Length == 0)
        {
            winUI.SetActive(true);
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
        if (alreadyWin) calculateData();
        StartCoroutine(Coroutine1());
    }

    IEnumerator Coroutine1()
    {
        // ランクアップ時はタップされたら次のシーンへ。(一つ前のタップが影響して誤判定されるのを防ぐために数秒待たせる)
        yield return new WaitForSeconds(0.5f);
        if (rankUp) yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        SceneManager.MoveGameObjectToScene(GameObject.Find("DataForRetry"), SceneManager.GetActiveScene());
        SceneManager.LoadScene("TitleScene");
    }


    // Nextボタン。DontDestroyOnloadを解除
    public void nextToPreparationScene()
    {
        calculateData();
        StartCoroutine(Coroutine2());
    }

    IEnumerator Coroutine2()
    {
        yield return new WaitForSeconds(0.5f);
        if (rankUp) yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        SceneManager.MoveGameObjectToScene(GameObject.Find("DataForRetry"), SceneManager.GetActiveScene());
        SceneManager.LoadScene("PreparationScene");
    }



    void calculateData()
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
        for (int i = 0; i < encycData.EncycList.Count; i++)
        {
            if (encycData.EncycList[i].RankInt <= rankInt) completionNum += 1;
        }
        encycCompletion = completionNum * 100 / encycData.EncycList.Count;


        // ランクが上がった時、新たに解放されたエネミー以外のキャラクター達を表示(フォントの大きさから4体まで)。
        if (rankInt != data.ArrivalRankInt)
        {

            for (int i = 0; i < encycData.EncycList.Count; i++)
            {
                if (encycData.EncycList[i].RankInt == rankInt && encycData.EncycList[i].MySide != "エネミー")
                {
                    releaseText.text += $"\n{encycData.EncycList[i].Name}";
                }
            }

            if (rankStr == "SS") maxRank.enabled = true;

            releaseUI.SetActive(true);
            rankUp = true;
        }


        // セーブデータに書き込み
        SaveData.SetSaveData(data.WinningStreak, rankStr, rankInt, restNum, encycCompletion);
    }

}