using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#region 説明
/*///

・リトライ用のデータ管理
 初挑戦時とリトライ時での動作を変える。リトライ時か否か(isRetried)はPreparationController、リトライ用のデータはDataForRetryが全ての呼び出し元になっている。

・コスト管理
 「①Difficulty設定→　②EnemyCostLimit設定→　③HomeCostLimit設定→　④各必要コストからTotalHomeCostの計算」の順に行われる。
 ①〜③はEnemySpawnControllerで管理。④は各ボタンで管理。全てのHomeCostLimit,TotalHomeCostの呼び出し元はPlayerPreparationになっている。

・プレイヤーステータス管理
 プレイヤーステータス管理は「①PlayerNum設定→　②MyStatusを設定→　③PlayerLevelingDataと各種初期ステータスを設定→
 ④各Up/Downボタンタップ時にステータス変動」の順に行われる。①〜③はPlayerChangeボタンで管理。④は各種ボタンで管理している。
 全てのPlayerNum,MyStatus,PlayerLevelingData,各Playerステータスの呼び出し元はPlayerPreparationになっている。

・キャラ配置管理
「」の順に行われる。

・読み込み優先
 ScriptExcutionOrderで読み込みを、PreparationController →　EnemySpawnController →　PlayerPreparation or AllocationControllerの順で優先

/*///
#endregion


public class EnemySpawnController: MonoBehaviour
{
    [SerializeField] PreparationController preparationController;
    [SerializeField] PlayerPreparation playerPreparation;
    [SerializeField] CharacterStatusData enemyStatusData;
    [SerializeField] SpawnPointData spawnPointData;
    [SerializeField] DifficultyData difficultyData;

    [SerializeField] DataForRetry dataForRetry;
    List<CharacterStatus> enemyList;
    List<Vector3> pointList;

    int enemyCostLimit;
    int numOfSpawn;
    int maxRange;
    int filledPointCount = 0;
    int homeCostLimit = 0;


    void Start()
    {
        dataForRetry = GameObject.Find("DataForRetry").GetComponent<DataForRetry>();

        // 初挑戦時とリトライ時で動作が違う。リトライ時は前回と同じ配置に。
        if (!preparationController.IsRetried)
        {
            enemyList = enemyStatusData.CharacterStatusList;
            pointList = spawnPointData.SpawnPointList[0].Point;

            playerPreparation.HomeCostLimit = spawn();

        }
        else
        {
            playerPreparation.HomeCostLimit = retrySpawn();
        }

    }


    #region 敵配置とHomeCostLimit設定の説明。
    // 敵はDifficultyレベルで定められたコストの範囲内で、ランダムな種類の敵が、ランダムの場所に、ランダムな数配置される。
    // そして配置後に、その時の敵の総使用コストが、HomeLimitCostとなる。

    // 流石にレベルデザインがクソすぎる。出てくるキャラもスライムとシェルスライムだけみたいなのから、勝っていくとドラゴンも出てくるようになるとか。コストに加えて、ランクを設定して、難易度１に対してはランク１以下の敵しか出ない、難易度２に対しては難易度２以下の敵が出るみたいにしよう。
    #endregion

    public int spawn()
    {

        // リストをシャッフル。ポジションと敵の種類の順番がランダムに。
        enemyList = enemyList.OrderBy(i => Guid.NewGuid()).ToList();
        pointList = pointList.OrderBy(i => Guid.NewGuid()).ToList();

        // Retry用に値渡し。
        dataForRetry.EnemyListForRe = new List<CharacterStatus>(enemyList);
        dataForRetry.PointListForRe = new List<Vector3>(pointList);
        dataForRetry.NumOfSpawnForRe = new List<int>();

        // Difficultyレベルで敵配置コストの範囲が決定。
        enemyCostLimit = difficultyData.DifficultyList[0].MaxCost;


        // 敵の種類数だけ繰り返し
        for (int i = 0; i < enemyList.Count; i++)
        {

            // 現在の敵配置コスト上限 ÷ ランダムに選ばれた敵の種類のコスト、で最大可能生成数を計算→　からの生成数をランダムで決定。
            // 保有コスト100、敵の種類スライム(コスト10)の時、最大で10匹生成。
            maxRange = enemyCostLimit / enemyList[i].Cost;
            numOfSpawn = UnityEngine.Random.Range(0, maxRange);

            // Retry用。
            dataForRetry.NumOfSpawnForRe.Add(numOfSpawn);


            // 生成数だけ繰り返し
            for (int p = 0; p < numOfSpawn; p++)
            {
                // spawnPointに空きがあり、配置しても敵配置コスト上限がマイナスにならない時
                if (filledPointCount != pointList.Count && 0 < enemyCostLimit - enemyList[i].Cost)
                {

                    Instantiate(enemyList[i].Prefab, pointList[filledPointCount], Quaternion.Euler(0, 180, 0));
                    
                    enemyCostLimit -= enemyList[i].Cost;
                    filledPointCount += 1;
                    homeCostLimit += enemyList[i].Cost;

                }
            }
        }
        return homeCostLimit;
    }



    public int retrySpawn()
    {
        // 敵の種類数だけ繰り返し
        for (int i = 0; i < dataForRetry.EnemyListForRe.Count; i++)
        {

            // 生成数だけ繰り返し
            for (int p = 0; p < dataForRetry.NumOfSpawnForRe[i]; p++)
            {
                Instantiate(dataForRetry.EnemyListForRe[i].Prefab, dataForRetry.PointListForRe[filledPointCount], Quaternion.Euler(0, 180, 0));

                filledPointCount += 1;
                homeCostLimit += dataForRetry.EnemyListForRe[i].Cost;
            }
        }
        return homeCostLimit;
    }


}
