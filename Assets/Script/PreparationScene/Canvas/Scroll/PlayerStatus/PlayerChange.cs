using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

// PlayerNum、MyStatusからのPlayerLevelingDataと各種ステータス、PlayerChangeボタンデザインの管理
public class PlayerChange : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] CharacterStatusData playerStatusData;
    [SerializeField] PlayerPreparation playerPreparation;
    [SerializeField] Image playerImage;
    [SerializeField] Text playerName;
    GameObject nowPrefab;


    private void Start()
    {
        // きつね犬をstart時のステータスに。
        playerPreparation.HpLv = 1;
        playerPreparation.AtkLv = 1;
        playerPreparation.SpdLv = 1;
        playerPreparation.AtkRateLv = 1;
        playerPreparation.MyStatus = playerStatusData.CharacterStatusList[0];
        setFirstStatus();
    }


    //　ボタンタップ時に、キャラクターが代わり、ステータスが初期値になる。
    public void OnPointerDown(PointerEventData eventData)
    {
        // PlayerNumの設定
        playerPreparation.PlayerNum += 1;
        // リスト内を循環させる
        if (playerStatusData.CharacterStatusList.Count <= playerPreparation.PlayerNum)
        {
            playerPreparation.PlayerNum = 0;
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

    }


    void setFirstStatus()
    {
        playerPreparation.PlayerLevelingData = playerPreparation.MyStatus.PlayerLevelingData;
        playerPreparation.PlayerHp = playerPreparation.MyStatus.Hp;
        playerPreparation.PlayerAtk = playerPreparation.MyStatus.Atk;
        playerPreparation.PlayerAtkRate = playerPreparation.MyStatus.AtkRate;
        playerPreparation.PlayerSpd = playerPreparation.MyStatus.Spd;
        nowPrefab = Instantiate(playerPreparation.MyStatus.Prefab);
    }

}
