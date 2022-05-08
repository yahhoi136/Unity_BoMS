using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

#region PreparatioinControllers説明
/*///

・リトライ用のデータ管理
 初挑戦時とリトライ時での動作を変える。リトライ時か否か(isRetried)はPreparationController、リトライ用のデータはDataForRetryが全ての呼び出し元になっている。

・コスト管理
 「①Difficulty設定→　②EnemyCostLimit設定→　③HomeCostLimit設定→　④各必要コストからTotalHomeCostの計算」の順に行われる。
 ①〜③はEnemySpawnControllerで管理。④は各ボタンで管理。全てのHomeCostLimit,TotalHomeCostの呼び出し元はPlayerPreparationになっている。

・PreparationSceneでのプレイヤーステータス管理
 PreparationSceneでのプレイヤーステータス管理は「①PlayerNum設定→　②MyStatusを設定→　③PlayerLevelingDataと各種初期ステータスを設定→
 ④各Up/Downボタンタップ時にステータス変動」の順に行われる。①〜③はPlayerChangeボタンで管理。④は各種ボタンで管理している。
 全てのPlayerNum,MyStatus,PlayerLevelingData,Preparationでの各Playerステータス,の呼び出し元はPlayerPreparationになっている。

・キャラ配置管理
「①新規キャラの配置/配置取り消し→　②既存キャラの移動/削除」の順に行われる。
 ①は各AllocationボタンとAllocationDeleteによって管理されており、②はAllocationControllerとAllocationDeleteによって管理されている。

・読み込み優先
 ScriptExcutionOrderで読み込みを、PreparationController →　EnemySpawnController →　PlayerPreparation or AllocationControllerの順で優先

/*///
#endregion

public class AllocationController : MonoBehaviour
{
    [SerializeField] PlayerPreparation playerPreparation;
    [SerializeField] AllocationDelete allocationDelete;
    GameObject homeChara;
    float x;
    float z;
    bool isButtonSelected;
    bool isPicked;


    void Update()
    {
        // 新規キャラ配置中以外で、
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            isButtonSelected = true;
            return;
        }

        // 配置されている味方キャラがマウスクリック＆ドラッグされた時、そのキャラをマウスカーソルに追従させる。
        if (Input.GetMouseButtonDown(0))
        {
            // (currentSelectedGameObject == null && GetMouseButtonDown)を判定に使うと、select→ 配置時のマウスクリックにも反応してしまう。
            if (isButtonSelected)
            {
                isButtonSelected = false;
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                int homeCharaLayerMask = 1 << 10;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, homeCharaLayerMask))
                {
                    homeChara = hit.collider.transform.root.gameObject;
                    isPicked = true;
                }
            }
        }


        if (isPicked)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int floorLayerMask = 1 << 9;
            
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, floorLayerMask))
            {
                x = hit.point.x;
                z = hit.point.z;
            }

            // HomeTerritoryの中だけを追従。
            if (-10 < x && x < 10 && -12 < z && z < 0) { homeChara.transform.position = new Vector3(x, 0.1f, z); }

            // キャラ移動中のみAllocationDeleteボタンが光る。
            allocationDelete.myButton.interactable = true;

            // 味方キャラ移動中に、AllocationDeleteボタンが押されると、配置を解除してその分コストを元に戻す。
            if (allocationDelete.isDeleted)
            {
                EventSystem.current.SetSelectedGameObject(null);
                isPicked = false;
                allocationDelete.myButton.interactable = false;
                playerPreparation.TotalHomeCost -= homeChara.GetComponent<StatusController>().cost;
                Destroy(homeChara);
            }

            if (Input.GetMouseButtonUp(0))
            {
                isPicked = false;
                allocationDelete.myButton.interactable = false;
            }
        }

    }
}
