using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

// UIレイアウトの自動伸縮をEncyclopediaと同期したいため、_DemoのプレハブはEncyclopediaの子オブジェクトとして作る。transformとかもそれを留意してね。
// ステータスを調節する時は、先にCharacterStatusを設定した後に、EncycDataを手動で適応させる。CharacterStatusDataとEncycDataは同期していない。
// Explanationは一つ一つ実際のテキストを入れてみて、句読点や括弧などの変な改行が行われないように、適宜スペースなどを入れて調節する。

public class Encyclopedia : MonoBehaviour
{
    [SerializeField] EncycData encycData;
    [SerializeField] Text Name;
    [SerializeField] Text MySide;
    [SerializeField] Text Page;
    [SerializeField] Text Status;
    [SerializeField] Text Explanation;
    [SerializeField] Button forwardButton;
    [SerializeField] Button backButton;
    SaveData data;
    int charaNum;
    Encyc encyc;
    GameObject nowDemoPrefab;


    
    private void Start()
    {
        // データのロード。データがないときはTitleControllerの方でボタンが無効化されててEncyclopediaは開けない。
        using (var reader = new StreamReader(Application.persistentDataPath + "/SaveData.json"))
        {
            JsonSerializer serializer = new JsonSerializer();
            data = (SaveData)serializer.Deserialize(reader, typeof(SaveData));
        }
    }


    // Encycが開かれる度に、1ページ目を表示
    private void OnEnable()
    {
        backButton.interactable = false;
        charaNum = 1;
        encyc = encycData.EncycList[0];

        Page.text = $"1 / {encycData.EncycList.Count}";
        display(encyc);
    }


    public void forward()
    {
        if (charaNum == encycData.EncycList.Count) return;
        
        charaNum += 1;
        if (charaNum == encycData.EncycList.Count) forwardButton.interactable = false;
        if (nowDemoPrefab) Destroy(nowDemoPrefab);
        backButton.interactable = true;

        encyc = encycData.EncycList[charaNum - 1];
        Page.text = $"{charaNum} / {encycData.EncycList.Count}";
        MySide.text = $"{encyc.MySide}";
        // 到達ランクがそのキャラのランク以上で表示。
        if(encyc.RankInt <= data.ArrivalRankInt)
        {
            display(encyc);
        }
        else
        {
            displayUnknown(encyc);
        }

    }


    public void back()
    {
        if (charaNum == 1) return;

        charaNum -= 1;
        if (charaNum == 1) backButton.interactable = false;
        if (nowDemoPrefab) Destroy(nowDemoPrefab);
        forwardButton.interactable = true;

        encyc = encycData.EncycList[charaNum - 1];
        Page.text = $"{charaNum} / {encycData.EncycList.Count}";
        MySide.text = $"{encyc.MySide}";
        // 到達ランクがそのキャラのランク以上で表示。
        if (encyc.RankInt <= data.ArrivalRankInt)
        {
            display(encyc);
        }
        else
        {
            displayUnknown(encyc);
        }

    }


    void display(Encyc encyc)
    {
        Name.text = encyc.Name;
        nowDemoPrefab = Instantiate(encyc.DemoPrefab, transform);
        Status.text = $" RANK    {encyc.RankStr}          COST   {encyc.Cost} \nHP       {encyc.Hp}            ATK   {encyc.Atk}\nATK/s   {encyc.AtkRate}            SPD  {encyc.Spd}";
        Explanation.text = encyc.Explanation;

    }

    void displayUnknown(Encyc encyc)
    {
        Name.text = "？？？";
        Status.text = " RANK    ー         COST   ー \nHP         ー            ATK   ー\nATK/s    ー            SPD   ー";
        Explanation.text = $"\n      〜  ランク {encyc.RankStr} 以上で解放  〜\n";
    }


    public void OnDisable()
    {
        Destroy(nowDemoPrefab);
        gameObject.SetActive(false);

    }

}
