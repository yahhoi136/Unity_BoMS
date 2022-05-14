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


    // デリゲートに追加。
    private void Start()
    {
        resetText();

        // デリゲートに追加。
        playerPreparation.resetLv += this.resetLv;
        playerPreparation.resetText += this.resetText;
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


 ///// AtkRateDownボタンが押されたとき。インスペクタで接続
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
                    // なぜか値が.9999999のようにバグるので四捨五入で調節。
                    playerPreparation.PlayerAtkRate = Mathf.Round((playerPreparation.PlayerAtkRate - myLeveling.Incre[i]) * 10) / 10;
                    playerPreparation.TotalHomeCost -= myLeveling.Cost[i];
                    incrementText.text = $"{playerPreparation.PlayerAtkRate}/s → {playerPreparation.PlayerAtkRate + myLeveling.Incre[i]}/s";
                    costText.text = $"COST {myLeveling.Cost[i]} ";

                    playerPreparation.AtkRateLv -= 1;
                }
            }
        }
    }

 ///// キャラクターが代わって全て初期値に戻った時
 ///
    void resetLv()
    {
        // レベルを１に戻してコストを全返却。
        while (playerPreparation.AtkRateLv > 1)
        {
            AtkRatedown();
        }
    }


    void resetText()
    {
        // AttackRateのレベリングデータ
        myLeveling = playerPreparation.PlayerLevelingData.playerLevelingList[3];

        // Textを初期値に
        incrementText.text = $"{playerPreparation.PlayerAtkRate}/s → {playerPreparation.PlayerAtkRate + myLeveling.Incre[0]}/s";
        costText.text = $"COST {myLeveling.Cost[0]} ";
    }


}
