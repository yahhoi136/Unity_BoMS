using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

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



public class EnemySpawnController : MonoBehaviour
{
    [SerializeField] PreparationController preparationController;
    [SerializeField] PlayerPreparation playerPreparation;
    [SerializeField] CharacterStatusData enemyStatusData;
    [SerializeField] SpawnPointData spawnPointData;
    [SerializeField] DifficultyData difficultyData;

    [SerializeField, NotEditable] DataForRetry dataForRetry;
    List<CharacterStatus> enemyList;
    List<Vector3> pointList;
    SaveData data;
    List<int> numOfSpawnList;


    void Start()
    {
        dataForRetry = GameObject.Find("DataForRetry").GetComponent<DataForRetry>();

        // 初挑戦時とリトライ時で動作が違う。初戦時。
        if (!preparationController.IsRetried)
        {

            enemyList = enemyStatusData.CharacterStatusList;
            pointList = spawnPointData.SpawnPointList[0].Point;

            // 配列をシャッフル
            enemyList = enemyList.OrderBy(i => Guid.NewGuid()).ToList();
            pointList = pointList.OrderBy(i => Guid.NewGuid()).ToList();
            setNumOfSpawn();

            // numOfSpawnListの数だけ、シャッフルされたenemyList, pointListから敵をスポーン。
            spawnEnemy(enemyList, pointList, numOfSpawnList);

            // Retry用に値渡し。
            dataForRetry.EnemyListForRe = new List<CharacterStatus>(enemyList);
            dataForRetry.PointListForRe = new List<Vector3>(pointList);
            dataForRetry.NumOfSpawnForRe = new List<int>(numOfSpawnList);
        }


        // リトライ時は前回と同じ配置に。
        else
        {
            spawnEnemy(dataForRetry.EnemyListForRe, dataForRetry.PointListForRe, dataForRetry.NumOfSpawnForRe);
        }

    }


    #region 敵配置とHomeCostLimit設定の説明。
    // 敵はDifficultyレベルで定められたCostとRankの範囲内で、ランダムな種類の敵が、ランダムの場所に、ランダムな数配置される。
    // そして配置後に、その時の敵の総使用コストが、HomeLimitCostとなる。
    // Difficultyによる難易度はMaxCostとRankの範囲で決められるが、Costの最低限保証がCost = 0 以外で存在しないため、
    // よりMaxCostに近い生成を可能にするためには(難易度に対して敵が少すぎなくするには)、Phase数を多くしてRankの範囲を広げて、ランダム生成のムラを無くす必要がある。

    #endregion


    // 難易度で決定された、敵のランク、総コストの範囲内かつ、空きポジションがある範囲内で敵のスポーン数を決定する。
    void setNumOfSpawn()
    {
        Difficulty difficulty;
        int maxCost = 0;
        int minRank = 0;
        int maxRank = 0;
        int maxRange;
        int maxSpawn;
        int emptyPointNum;
        int totalCost = 0;
        int numOfSpawn = 0;


        // セーブデータ読み込み
        using (var reader = new StreamReader(Application.persistentDataPath + "/SaveData.json"))
        {
            JsonSerializer serializer = new JsonSerializer();
            data = (SaveData)serializer.Deserialize(reader, typeof(SaveData));
        }


        // 難易度を現在のプレイヤーランクに設定
        for (int i = 0; i < difficultyData.DifficultyList.Count; i++)
        {
            if (data.ArrivalRankStr == difficultyData.DifficultyList[i].RankStr)
            {
                difficulty = difficultyData.DifficultyList[i];

                // 難易度から配置する敵のランクと総コストの範囲が決定。
                maxCost = difficulty.MaxCost;
                minRank = difficulty.MinRank;
                maxRank = difficulty.MaxRank;
            }
        }

        emptyPointNum = pointList.Count;

        // 敵キャラ0の時はやり直し。
        while (totalCost == 0)
        {
            // 初期化
            totalCost = 0;
            numOfSpawnList = new List<int>();

            // 全ての敵の種類について
            for (int i = 0; i < enemyList.Count; i++)
            {
                // 敵ランクが範囲外の時、スポーン数0にして次へ
                if (!(minRank <= enemyList[i].RankInt && enemyList[i].RankInt <= maxRank))
                {
                    numOfSpawnList.Add(0);
                    continue;
                }

                // 現在のコスト上限 ÷ 選ばれた敵の種類のコスト、で最大可能生成数を計算。そして生成数をランダムで決定。
                maxRange = (maxCost - totalCost) / enemyList[i].Cost;
                maxSpawn = UnityEngine.Random.Range(0, maxRange);

                // ただし、スポーン地点の空きの数だけ配置可能
                if (0 <= emptyPointNum - maxSpawn)
                {
                    numOfSpawn = maxSpawn;
                    totalCost += enemyList[i].Cost * numOfSpawn;
                    emptyPointNum -= maxSpawn;
                }
                else
                {
                    numOfSpawn = emptyPointNum;
                    totalCost += enemyList[i].Cost * numOfSpawn;
                    emptyPointNum = 0;
                }

                // 配置数をリスト化して記録
                numOfSpawnList.Add(numOfSpawn);
            }
        }
    }


    // 敵の情報とスポーン数は同じ順番で同じ要素数リストにされているため、検索不要。
    // enemyList[敵Aの情報, 敵Cの情報, 敵Bの情報...] <-> numOfSpawnList[敵Aのスポーン数, 敵Cのスポーン数, 敵Bのスポーン数...]
    void spawnEnemy(List<CharacterStatus> randomEnemyList, List<Vector3> randomPointList, List<int> numOfSpawnList)
    {
        int filledPointCount = 0;
        // 敵の種類数だけ繰り返し
        for (int i = 0; i < randomEnemyList.Count; i++)
        {

            // 生成数だけ繰り返し
            for (int p = 0; p < numOfSpawnList[i]; p++)
            {
                Instantiate(randomEnemyList[i].Prefab, randomPointList[filledPointCount], Quaternion.Euler(0, 180, 0));

                filledPointCount += 1;
                playerPreparation.HomeCostLimit += randomEnemyList[i].Cost;
            }
        }
    }

}
