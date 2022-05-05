using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AttackRateChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{

    [SerializeField] PlayerPreparation playerPreparation;
    [SerializeField] Text costText;
    [SerializeField] Text incrementText;
    [SerializeField] GameObject myDownButton;
    [SerializeField] float disappearTime = 0.1f;

    PlayerLeveling myLeveling;

    private void Start()
    {
        // retry用にコピーされた時は、Startの重複を避ける
        if (!GameObject.FindGameObjectWithTag("CopyForRetry"))
        {
            // AttackRateのレベリングデータ
            myLeveling = playerPreparation.PlayerLevelingData.playerLevelingList[3];

            // AttackRateUpボタン初期デザイン
            incrementText.text = $"{playerPreparation.PlayerAtkRate}/s → {playerPreparation.PlayerAtkRate + myLeveling.Incre[0]}/s";
            costText.text = $"COST {myLeveling.Cost[0]} ";
        }
    }


 ///// AttackRateUpボタンの上にカーソルが来た時
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


 ///// AttackRateUpボタンが押されたとき
 ///
    public void OnPointerDown(PointerEventData eventData)
    {
        if (playerPreparation.AtkRateLv < myLeveling.MaxLv)
        {
            // レベル間AtkRate増分を加えて、レベル間必要コストをTotalHomeCostに足している
            for (int i = 0; i < myLeveling.MaxLv; i++)
            {
                if (playerPreparation.AtkRateLv == i + 1)
                {
                    playerPreparation.PlayerAtkRate += myLeveling.Incre[i];
                    playerPreparation.TotalHomeCost += myLeveling.Cost[i];

                    if (myLeveling.MaxLv == i + 2)
                    {
                        incrementText.text = $"  {playerPreparation.PlayerAtkRate}/s  Max！";
                        costText.text = "COST ー ";
                    }
                    else
                    {
                        incrementText.text = $"{playerPreparation.PlayerAtkRate}/s → {playerPreparation.PlayerAtkRate + myLeveling.Incre[i + 1]}/s";
                        costText.text = $"COST {myLeveling.Cost[i + 1]} ";
                    }

                    playerPreparation.AtkRateLv += 1;
                    return;
                }
            }
        }
    }


 ///// AtkRateDownボタンが押されたとき
 ///
    public void AtkRatedown()
    {
        if (playerPreparation.AtkRateLv > 1)
        {

            // レベル間AtkRate増分を引いて、レベル間必要コストをTotalHomeCostから引いている
            for (int i = 0; i < myLeveling.MaxLv; i++)
            {
                if (playerPreparation.AtkRateLv == i + 2)
                {
                    playerPreparation.PlayerAtkRate -= myLeveling.Incre[i];
                    playerPreparation.TotalHomeCost -= myLeveling.Cost[i];

                    incrementText.text = $"{playerPreparation.PlayerAtkRate}/s → {playerPreparation.PlayerAtkRate + myLeveling.Incre[i]}/s";
                    costText.text = $"COST {myLeveling.Cost[i]} ";

                    playerPreparation.AtkRateLv -= 1;
                }
            }
        }
    }


}
