using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class RankInfo : MonoBehaviour
{
    [SerializeField] Text nowRank;
    [SerializeField] Text nextRank;
    SaveData data;
 
    void Start()
    {
        try
        {
            // セーブデータ読み込み
            using (var reader = new StreamReader(Application.persistentDataPath + "/SaveData.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                data = (SaveData)serializer.Deserialize(reader, typeof(SaveData));
            }
        }
        // ファイルがない場合
        catch (FileNotFoundException)
        {
            print("ファイル無いよ");
            print(Application.persistentDataPath);
        }

        nowRank.text = $"{data.HighestArrival}";
        nextRank.text = $"次ランクまで\n あと{data.RestNum} 連勝！";
    }


}
