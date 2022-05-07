using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

#region セーブデータ説明
/*///

・セーブデータ
 セーブデータのクラスと書き込みメソッドの宣言はSaveDataで、セーブ内容のWinningStreak、HighestArrival、Completionの設定はBattleControllerで行っている。

/*///
#endregion


public class SaveData
{

    [JsonProperty("連勝数")]
    public int WinningStreak { get; set; }
    [JsonProperty("最高到達ランク")]
    public string HighestArrival { get; set; }
    [JsonProperty("次ランクまでの勝利数")]
    public int RestNum { get; set; }
    [JsonProperty("図鑑完成率")]
    public string Completion { get; set; }


    // newでJson作ります→　シリアライズ化します→　書き込みます。
    // デシリアライズしながら読み込みます

    public static void SetSaveData(int winningStreak,string highestArrival,int restNum, string completion)
    {
        var data = new SaveData()
        {
            WinningStreak = winningStreak,
            HighestArrival = $"{highestArrival}",
            RestNum = restNum,
            Completion = $"{completion}",
        };

        File.WriteAllText(Application.persistentDataPath + "/SaveData.json", JsonConvert.SerializeObject(data, Formatting.Indented));
    }
}