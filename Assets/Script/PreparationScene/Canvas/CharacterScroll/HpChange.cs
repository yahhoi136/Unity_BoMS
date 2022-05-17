using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HpChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{

    [SerializeField] PlayerPreparation playerPreparation;
    [SerializeField] Text costText;
    [SerializeField] Text incrementText;
    [SerializeField] GameObject myDownButton;
    [SerializeField] float disappearTime = 0.1f;
    PlayerLeveling myLeveling;


    private void Start()
    {
        resetText();

        // デリゲートに追加。
        playerPreparation.resetLv += this.resetLv;
        playerPreparation.resetText += this.resetText;
    }


 ///// HpUpボタンの上にカーソルが来た時
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


 ///// HpUpボタンが押されたとき
 ///
    public void OnPointerDown(PointerEventData eventData)
    {
        if (playerPreparation.HpLv < myLeveling.MaxLv)
        {
            // レベル間Hp増分を加えて、レベル間必要コストをTotalHomeCostに足している
            for (int i = 0; i < myLeveling.MaxLv; i++)
            {
                if(playerPreparation.HpLv == i+1)
                {
                    playerPreparation.PlayerHp += myLeveling.Incre[i];
                    playerPreparation.TotalHomeCost += myLeveling.Cost[i];
                    
                    if (myLeveling.MaxLv == i+2)
                    {
                        incrementText.text = $"HP{playerPreparation.PlayerHp} Max！";
                        costText.text = "COST ー ";
                    }
                    else
                    {
                        incrementText.text = $"HP{playerPreparation.PlayerHp} → {playerPreparation.PlayerHp + myLeveling.Incre[i + 1]} ";
                        costText.text = $"COST {myLeveling.Cost[i + 1]} ";
                    }

                    playerPreparation.HpLv += 1;
                    return;
                }
            }
        }
    }


 ///// HpDownボタンが押されたとき。インスペクタで接続
 ///
    public void Hpdown()
    {
        if (playerPreparation.HpLv > 1)
        {

            // レベル間Hp増分を引いて、レベル間必要コストをTotalHomeCostから引いている
            for (int i = 0; i < myLeveling.MaxLv; i++)
            {
                if (playerPreparation.HpLv == i+2)
                {
                    playerPreparation.PlayerHp -= myLeveling.Incre[i];
                    playerPreparation.TotalHomeCost -= myLeveling.Cost[i];

                    incrementText.text = $"HP{playerPreparation.PlayerHp} → {playerPreparation.PlayerHp + myLeveling.Incre[i]} ";
                    costText.text = $"COST {myLeveling.Cost[i]} ";

                    playerPreparation.HpLv -= 1;
                }
            }
        }
    }


 ///// キャラクターが代わって全て初期値に戻った時
 ///
    void resetLv()
    {
        // レベルを１に戻してコストを全返却。
        while (playerPreparation.HpLv > 1)
        {
            Hpdown();
        }
    }

    void resetText()
    {
        // Hpのレベリングデータ
        myLeveling = playerPreparation.PlayerLevelingData.playerLevelingList[0];

        // テキストを初期値に。
        incrementText.text = $"HP{playerPreparation.PlayerHp} → {playerPreparation.PlayerHp + myLeveling.Incre[0]} ";
        costText.text = $"COST {myLeveling.Cost[0]} ";
    }


}
