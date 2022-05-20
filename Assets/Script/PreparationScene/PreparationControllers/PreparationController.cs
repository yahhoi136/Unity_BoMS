using UnityEngine;
using UnityEngine.SceneManagement;

#region PreparatioinControllers説明
/*///

・リトライ用のデータ管理
 初挑戦時とリトライ時での動作を変える。リトライ時か否か(isRetried)はPreparationController、リトライ用のデータはDataForRetryが全ての呼び出し元になっている。

・コストと敵キャラ配置管理
 「①Difficulty設定→　②EnemyCostLimit設定→　③HomeCostLimit設定→　④各必要コストからTotalHomeCostの計算」の順に行われる。
 ①〜③はEnemySpawnControllerで管理。④は各ボタンで管理。全てのHomeCostLimit,TotalHomeCostの呼び出し元はPlayerPreparationになっている。
 ちなみにDifficultyDataのMinRankとMaxRankの差は0〜1程度にしておかないとsetNumOfSpawn()の中のwhile文が永遠に終わらなくてバグる。

・PreparationSceneでのプレイヤーステータス管理(戦闘開始時初期プレイヤーステータスの設定)
 「①PlayerNum設定→　②MyStatusを設定→　③PlayerLevelingDataと各種初期ステータスを設定→④各Up/Downボタンタップ時にステータス変動」の順に行われる。
 ①〜③はPlayerChangeボタンで管理。④は各種ボタンで管理している。全てのPlayerNum,MyStatus,PlayerLevelingData,Preparationでの各Playerステータス,
 の呼び出し元はPlayerPreparationになっている。
 ちなみに、AtkRateとは、1秒間の攻撃回数になっている。全てのキャラは1秒に1回だけ攻撃アニメーションをするように(1度に2回攻撃するものは2秒に1回だけ)、
 Animatorで調節しており、その倍数のMultiplierとしてAtkrateで攻撃速度を設定できる。

・味方キャラ配置管理
「①新規キャラの配置/配置取り消し→　②既存キャラの移動/削除」の順に行われる。
 ①は各AllocationボタンとAllocationDeleteによって管理されており、②はAllocationControllerとAllocationDeleteによって管理されている。
 
・読み込み優先
 ScriptExcutionOrderで読み込みを、PreparationController →　EnemySpawnController →　PlayerPreparation or AllocationControllerの順で優先

/*///
#endregion


public class PreparationController : MonoBehaviour
{
    [SerializeField] GameObject encycBGI;
    [SerializeField] GameObject encyclopedia;
    [SerializeField, NotEditable] GameObject go;
    bool _isRetried;
    public bool IsRetried { get { return _isRetried; } set { _isRetried = value; } }

    void Start()
    {
        // Retry用にDataForRetryオブジェクトにデータを保存。
        if (!GameObject.FindGameObjectWithTag("Retried"))
        {
            go = new GameObject();
            go.name = "DataForRetry";
            go.AddComponent<DataForRetry>();
            DontDestroyOnLoad(go);

            IsRetried = false;
        }
        else
        {
            go = GameObject.FindGameObjectWithTag("Retried");
            IsRetried = true;
        }
    }


    public void openEncyclopedia()
    {
        if (encyclopedia.activeInHierarchy)
        {
            encycBGI.SetActive(false);
            encyclopedia.SetActive(false);
        }
        else
        {
            encycBGI.SetActive(true);
            encyclopedia.SetActive(true);
        }
    }


    public void battleStart()
    {
        SceneManager.LoadScene("BattleScene");
    }

}
