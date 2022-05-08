using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

// PlayerNum、MyStatusからのPlayerLevelingDataと各種ステータス、PlayerChangeボタンデザインの管理
public class PlayerChange : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] CharacterStatusData characterStatusData;
    [SerializeField] PlayerPreparation playerPreparation;
    [SerializeField] Image playerImage;
    [SerializeField] Text playerName;


    private void Start()
    {
        // tag で"Retried"で　リトライ時か判断する?正直他のパラメは初期値に戻してる。リセットしたい時とかにそっちの方が手っ取り早そうだから。配置キャラ一々AllocationDeleteまで持っていかないと行けないしね。
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // PlayerNumの設定
        playerPreparation.PlayerNum += 1;
        // リスト内を循環させる
        if (characterStatusData.CharacterStatusList.Count <= playerPreparation.PlayerNum)
        {
            playerPreparation.PlayerNum = 0;
        }

        // MyStatusからのPlayerLevelingDataと各種ステータス設定
        playerPreparation.MyStatus = characterStatusData.CharacterStatusList[playerPreparation.PlayerNum];
        playerPreparation.PlayerLevelingData = playerPreparation.MyStatus.PlayerLevelingData;
        playerPreparation.PlayerHp = playerPreparation.MyStatus.Hp;
        playerPreparation.PlayerAtk = playerPreparation.MyStatus.Atk;
        playerPreparation.PlayerAtkRate = playerPreparation.MyStatus.AtkRate;
        playerPreparation.PlayerSpd = playerPreparation.MyStatus.Spd;


        // PlayerChangeボタンデザインの管理
        playerImage.sprite = playerPreparation.MyStatus.PlayerSprite;
        playerName.text = playerPreparation.MyStatus.Name;

    }

}
