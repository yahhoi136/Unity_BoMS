using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpeedChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{

    [SerializeField] PlayerPreparation playerPreparation;
    [SerializeField] Text costText;
    [SerializeField] Text incrementText;
    [SerializeField] GameObject myDownButton;
    [SerializeField] float disappearTime = 0.1f;

    PlayerLeveling myLeveling;

    private void Start()
    {
        // Speedのレベリングデータ
        myLeveling = playerPreparation.PlayerLevelingData.playerLevelingList[2];

        // SpeedUpボタン初期デザイン
        incrementText.text = $"{playerPreparation.PlayerSpd}m/s → {playerPreparation.PlayerSpd + myLeveling.Incre[0]}m/s";
        costText.text = $"COST {myLeveling.Cost[0]} ";
    }


 ///// SpeedUpボタンの上にカーソルが来た時
 ///
    #region 説明
    // Upボタンに触れた時だけ、そのDownボタンが表示される。DownボタンはUpボタンの子オブジェなのでDownに触れてる時も表示される。
    // UpボタンからDownボタンに移動する際に一瞬隙間があるので、その部分を移動する時間だけ処理を遅らせる。
    #endregion

    public void OnPointerEnter(PointerEventData eventData)
    {
        myDownButton.SetActive(true);
        CancelInvoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Invoke(nameof(disappearDelete), disappearTime);
    }

    void disappearDelete()
    {
        myDownButton.SetActive(false);
    }


 ///// SpeedUpボタンが押されたとき
 ///
    public void OnPointerDown(PointerEventData eventData)
    {
        if (playerPreparation.SpdLv < myLeveling.MaxLv)
        {
            // レベル間Spd増分を加えて、レベル間必要コストをTotalHomeCostに足している
            for (int i = 0; i < myLeveling.MaxLv; i++)
            {
                if (playerPreparation.SpdLv == i + 1)
                {
                    playerPreparation.PlayerSpd += myLeveling.Incre[i];
                    playerPreparation.TotalHomeCost += myLeveling.Cost[i];

                    if (myLeveling.MaxLv == i + 2)
                    {
                        incrementText.text = $" {playerPreparation.PlayerSpd}m/s Max！";
                        costText.text = "COST ー ";
                    }
                    else
                    {
                        incrementText.text = $"{playerPreparation.PlayerSpd}m/s → {playerPreparation.PlayerSpd + myLeveling.Incre[i + 1]}m/s";
                        costText.text = $"COST {myLeveling.Cost[i + 1]} ";
                    }

                    playerPreparation.SpdLv += 1;
                    return;
                }
            }
        }
    }


 ///// SpdDownボタンが押されたとき
 ///
    public void Spddown()
    {
        if (playerPreparation.SpdLv > 1)
        {

            // レベル間Spd増分を引いて、レベル間必要コストをTotalHomeCostから引いている
            for (int i = 0; i < myLeveling.MaxLv; i++)
            {
                if (playerPreparation.SpdLv == i + 2)
                {
                    playerPreparation.PlayerSpd -= myLeveling.Incre[i];
                    playerPreparation.TotalHomeCost -= myLeveling.Cost[i];

                    incrementText.text = $"{playerPreparation.PlayerSpd}m/s → {playerPreparation.PlayerSpd + myLeveling.Incre[i]}m/s";
                    costText.text = $"COST {myLeveling.Cost[i]} ";

                    playerPreparation.SpdLv -= 1;
                }
            }
        }
    }


}
