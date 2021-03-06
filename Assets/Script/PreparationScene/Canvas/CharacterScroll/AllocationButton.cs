using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class AllocationButton: MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler
{
    [SerializeField] PlayerPreparation playerPreparation;
    [SerializeField] CharacterStatusData homeStatusData;
    [SerializeField] int characterNum;
    [SerializeField] AllocationDelete allocationDelete;
    [SerializeField] GameObject myPrefab;
    [SerializeField] Text costText;
    [SerializeField] Text inspectorNameText;
    [SerializeField] Text inspectorOtherText;
    SaveData data;
    GameObject createdPrefab;
    bool isSelected;
    bool isCreated;


    void Start()
    {
        // データのロード。
        using (var reader = new StreamReader(Application.persistentDataPath + "/SaveData.json"))
        {
            JsonSerializer serializer = new JsonSerializer();
            data = (SaveData)serializer.Deserialize(reader, typeof(SaveData));
        }

        // 自身の到達ランクが、このキャラのランクよりも低い時、このキャラの配置ボタンを非表示に。
        if (data.ArrivalRankInt < homeStatusData.CharacterStatusList[characterNum].RankInt) gameObject.SetActive(false);

        costText.text = $"COST {homeStatusData.CharacterStatusList[characterNum].Cost} ";
    }


    void Update()
    {
        // ボタンが選択されている時、マウスカーソルの位置に配置キャラを追従＆コスト消費、追従させる位置はHomeTerritoryの範囲内
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
                if (-10 < x && x < 10 && -12 < z && z < -1)
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
                playerPreparation.TotalHomeCost -= homeStatusData.CharacterStatusList[characterNum].Cost;
                Destroy(createdPrefab);
            }


            // またInspectorをボタン選択のキャラに。
            inspectorNameText.text = $"ステータス\n【{homeStatusData.CharacterStatusList[characterNum].Name}】";
            inspectorOtherText.text = $"HP                  {homeStatusData.CharacterStatusList[characterNum].Hp}\nATK                  {homeStatusData.CharacterStatusList[characterNum].Atk}\nATK RATE     {homeStatusData.CharacterStatusList[characterNum].AtkRate}/s\nSPEED          {homeStatusData.CharacterStatusList[characterNum].Spd}m/s";
        }
    }


    void create()
    {
        createdPrefab = Instantiate(myPrefab);
        playerPreparation.TotalHomeCost += homeStatusData.CharacterStatusList[characterNum].Cost;
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


    // ボタンの上にカーソルがきた時も、Inspectorをボタンのキャラに。
    public void OnPointerEnter(PointerEventData eventData)
    {
        inspectorNameText.text = $"ステータス\n【{homeStatusData.CharacterStatusList[characterNum].Name}】";
        inspectorOtherText.text = $"HP                  {homeStatusData.CharacterStatusList[characterNum].Hp}\nATK                  {homeStatusData.CharacterStatusList[characterNum].Atk}\nATK RATE     {homeStatusData.CharacterStatusList[characterNum].AtkRate}/s\nSPEED          {homeStatusData.CharacterStatusList[characterNum].Spd}m/s";

    }
}
