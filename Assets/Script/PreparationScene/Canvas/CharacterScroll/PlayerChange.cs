using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;

// PlayerNum、MyStatusからのPlayerLevelingDataと各種ステータス、PlayerChangeボタンデザインの管理
public class PlayerChange : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    [SerializeField] CharacterStatusData playerStatusData;
    [SerializeField] PlayerPreparation playerPreparation;
    [SerializeField] Image playerImage;
    [SerializeField] Text playerName;
    [SerializeField] Image changeImage;
    [SerializeField] Text inspectorNameText;
    [SerializeField] Text inspectorOtherText;
    SaveData data; 
    GameObject nowPrefab;


    private void Start()
    {
        // データのロード。
        using (var reader = new StreamReader(Application.persistentDataPath + "/SaveData.json"))
        {
            JsonSerializer serializer = new JsonSerializer();
            data = (SaveData)serializer.Deserialize(reader, typeof(SaveData));
        }

        // きつね犬をstart時のステータスに。
        playerPreparation.HpLv = 1;
        playerPreparation.AtkLv = 1;
        playerPreparation.SpdLv = 1;
        playerPreparation.AtkRateLv = 1;
        playerPreparation.MyStatus = playerStatusData.CharacterStatusList[0];
        setFirstStatus();

        // 自身の到達ランクが、きつね犬以外のプレイヤーキャラのランクより低い時、変更可能イメージを非表示に。
        if (data.ArrivalRankInt < playerStatusData.CharacterStatusList[1].RankInt)
        { changeImage.enabled = false; } else { changeImage.enabled = true; }

    }


    // ボタンタップ時に、プレイヤーキャラが代わり、ステータスがそのキャラの初期値になる。
    public void OnPointerDown(PointerEventData eventData)
    {
        // 自身の到達ランクが、きつね犬以外のプレイヤーキャラのランクより低い時、何もしない。
        if (data.ArrivalRankInt < playerStatusData.CharacterStatusList[1].RankInt) return;


        // リストの終わりにきたら、リストの初めに戻る。
        if (playerStatusData.CharacterStatusList.Count == playerPreparation.PlayerNum + 1)
        {
            playerPreparation.PlayerNum = 0;
        }
        else
        {
            // 自身の到達ランクが、次のプレイヤーキャラのランクより低い時、それ以降をスキップしてリストの初めに戻る。
            // (プレイヤーキャラはランクが低い順にList化されている)
            if (data.ArrivalRankInt < playerStatusData.CharacterStatusList[playerPreparation.PlayerNum + 1].RankInt)
            {
                playerPreparation.PlayerNum = 0;
            }
            else
            {
                playerPreparation.PlayerNum += 1;
            }
        }


        // 既存のキャラ削除して、コストとレベルをリセットして、MyStatusからのPlayerLevelingDataと各種ステータス設定、最後にその設定をテキストに反映。
        Destroy(nowPrefab);
        playerPreparation.resetLv();
        playerPreparation.MyStatus = playerStatusData.CharacterStatusList[playerPreparation.PlayerNum];
        setFirstStatus();
        playerPreparation.resetText();

        // PlayerChangeボタンデザインの管理
        playerImage.sprite = playerPreparation.MyStatus.PlayerSprite;
        playerName.text = playerPreparation.MyStatus.Name;

        reloadInspector();
    }


    void setFirstStatus()
    {
        playerPreparation.PlayerLevelingData = playerPreparation.MyStatus.PlayerLevelingData;
        playerPreparation.PlayerName = playerPreparation.MyStatus.Name;
        playerPreparation.PlayerHp = playerPreparation.MyStatus.Hp;
        playerPreparation.PlayerAtk = playerPreparation.MyStatus.Atk;
        playerPreparation.PlayerAtkRate = playerPreparation.MyStatus.AtkRate;
        playerPreparation.PlayerSpd = playerPreparation.MyStatus.Spd;
        playerPreparation.PlayerGravity = playerPreparation.MyStatus.Gravity;
        nowPrefab = Instantiate(playerPreparation.MyStatus.Prefab);
    }


    // ボタンの上にカーソルが来た時, Inspectorをプレイヤーのものに更新。
    public void OnPointerEnter(PointerEventData eventData)
    {
        reloadInspector();
    }

    void reloadInspector()
    {
        inspectorNameText.text = $"ステータス\n【{playerPreparation.PlayerName}】";
        inspectorOtherText.text = $"HP                  {playerPreparation.PlayerHp}\nATK                  {playerPreparation.PlayerAtk}\nATK RATE     {playerPreparation.PlayerAtkRate}/s\nSPEED          {playerPreparation.PlayerSpd}m/s";
    }
}
