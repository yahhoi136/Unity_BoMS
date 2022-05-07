using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleController : MonoBehaviour
{
    [SerializeField] DifficultyData difficultyData;
    [SerializeField] GameObject saveData;
    [SerializeField] Text dataText;
    [SerializeField] Text explanation;
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

        }

    }


    public void newGame()
    {
        saveData.SetActive(true);

        if (isDataExsist)
        {
            dataText.text = $"最高到達ランク　{data.HighestArrival}\n\n 図鑑完成率        {data.Completion} % ";
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
        saveData.SetActive(true);
        isLoadData = true;

        if (isDataExsist)
        {
            dataText.text = $"最高到達ランク　{data.HighestArrival}\n\n 図鑑完成率        {data.Completion} % ";
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
        if (isDataExsist && isLoadData)
        {
            SceneManager.LoadScene("PreparationScene");
        }
        else
        {
            // 初めから用のデータを作る。
            SaveData.SetSaveData(0, difficultyData.DifficultyList[0].Rank, difficultyData.DifficultyList[0].NumOfPhase, "0");
            SceneManager.LoadScene("PreparationScene");
        }
    }


    public void backToTitle()
    {
        saveData.SetActive(false);
        isLoadData = false;
    }


    public void encyclopedia()
    {

    }
}
