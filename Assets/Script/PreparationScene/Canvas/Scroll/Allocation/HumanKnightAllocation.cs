using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HumanKnightAllocation : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] PlayerPreparation playerPreparation;
    [SerializeField] CharacterStatusData homeStatusData;
    [SerializeField] AllocationDelete allocationDelete;
    [SerializeField] GameObject myPrefab;
    [SerializeField] Text costText;
    GameObject createdPrefab;
    bool isSelected;
    bool isCreated;


    void Start()
    {
        costText.text = $"COST {homeStatusData.CharacterStatusList[0].Cost} ";
    }


    void Update()
    {
        // ボタンが選択されている時、マウスカーソルの位置に配置キャラを追従＆コスト消費、追従させる位置はHomeTerritoryの範囲内。
        if (isSelected)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int floorMask = 1 << 9;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, floorMask))
            {
                // カーソルがHomeTerritory上にきて初めて複製を作る。限定しないとスクロールバー上で配置キャラを切り替えるだけでどんどん生産される。
                if (!isCreated) create();

                float x = hit.point.x;
                float z = hit.point.z;

                // HomeTerritoryの中だけを追従。
                if (-10 < x && x < 10 && -12 < z && z < 0)
                {
                    createdPrefab.transform.position = new Vector3(x, 0.1f, z);
                }

                // キャラ移動中のみAllocationDeleteボタンが光る。
                allocationDelete.myButton.interactable = true;
            }

            // ボタン選択中に、AllocationDeleteボタンが押されると、ボタンの選択と配置を解除してコストも元に戻す。
            if (allocationDelete.isDeleted && isCreated)
            {
                EventSystem.current.SetSelectedGameObject(null);
                isSelected = false;
                isCreated = false;
                allocationDelete.myButton.interactable = false;
                playerPreparation.TotalHomeCost -= homeStatusData.CharacterStatusList[0].Cost;
                Destroy(createdPrefab);
            }

        }
    }


    void create()
    {
        createdPrefab = Instantiate(myPrefab);
        playerPreparation.TotalHomeCost += homeStatusData.CharacterStatusList[0].Cost;
        isCreated = true;
    }


    // ボタン選択した時
    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
    }


    // ボタン選択が解除された時
    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
        isCreated = false;
        allocationDelete.myButton.interactable = false;
    }

}
