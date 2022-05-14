using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleController : MonoBehaviour
{
    [SerializeField] DifficultyData difficultyData;
    [SerializeField] GameObject saveDataUI;
    [SerializeField] Text dataText;
    [SerializeField] Text explanation;
    [SerializeField] GameObject cautionUI;
    [SerializeField] Button encycButton;
    [SerializeField] GameObject encyclopedia;
    SaveData data;
    bool isDataExsist;
    bool isLoadData;

    private void Start()
    {
        try
        {
            // セーブデータ読み込み
            using (var reader = new StreamReader(Application.persistentDataPath + "/SaveData.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                data = (SaveData)serializer.Deserialize(reader, typeof(SaveData));
                isDataExsist = true;
            }

        }
        // ファイルがない場合
        catch (FileNotFoundException)
        {
            isDataExsist = false;
            print("ファイル無いよ");
            print(Application.persistentDataPath);
            encycButton.interactable = false;
        }

    }


    public void newGame()
    {
        saveDataUI.SetActive(true);
        encycButton.interactable = false;

        if (isDataExsist)
        {
            dataText.text = $"最高到達ランク   {data.ArrivalRankStr}\n\n 図鑑完成率        {data.EncycCompletion} %";
            explanation.text = "このデータを削除して初めからにしますか？";

        }
        else
        {
            dataText.text = "最高到達ランク　ー\n\n 図鑑完成率      ー ー ％";
            explanation.text = "データがありません。初めからにしますか？";
        }

    }


    public void loadGame()
    {
        saveDataUI.SetActive(true);
        isLoadData = true;
        encycButton.interactable = false;

        if (isDataExsist)
        {
            dataText.text = $"最高到達ランク　{data.ArrivalRankStr}\n\n 図鑑完成率        {data.EncycCompletion} % ";
            explanation.text = "このデータをロードしますか？";
        }
        else
        {
            dataText.text = "最高到達ランク　ー\n\n 図鑑完成率      ー ー ％";
            explanation.text = "データがありません。初めからにしますか？";
        }
    }


    public void yesButton()
    {
        encycButton.interactable = false;

        if (isDataExsist && isLoadData)
        {
            SceneManager.LoadScene("PreparationScene");
        }
        else
        {
            // CautionUIを表示して、さらにイエスだと初めから用のデータを作る。
            cautionUI.SetActive(true);
        }
    }


    public void deleteYesButton()
    {
        // 初めから用のデータを作る。
        SaveData.SetSaveData(0, difficultyData.DifficultyList[0].RankStr, difficultyData.DifficultyList[0].RankInt, difficultyData.DifficultyList[0].PhaseNum, 0);
        SceneManager.LoadScene("PreparationScene");
    }


    public void backToTitle()
    {
        saveDataUI.SetActive(false);
        cautionUI.SetActive(false);
        isLoadData = false;

        if (isDataExsist) encycButton.interactable = true;

    }


    public void openEncyclopedia()
    {
        if (encyclopedia.activeInHierarchy)
        {
            encyclopedia.SetActive(false);
        }
        else
        {
            encyclopedia.SetActive(true);
        }
    }
}
