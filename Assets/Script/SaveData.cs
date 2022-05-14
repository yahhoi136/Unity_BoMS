using System.IO;
using Newtonsoft.Json;
using UnityEngine;

#region セーブデータ説明
/*///

・セーブデータ
 セーブデータのクラスと書き込みメソッドの宣言はSaveDataで、セーブ内容のWinningStreak、HighestArrival、Completionの書き込みはBattleControllerで行っている。

・ランク
 ランクはD→ C→ B→ A→ Sとなっており、数字で表される場合は、Dから順に1〜5となっている。

/*///
#endregion


public class SaveData
{

    [JsonProperty("連勝数")]
    public int WinningStreak { get; set; }
    [JsonProperty("到達ランク(文字列)")]
    public string ArrivalRankStr { get; set; }
    [JsonProperty("到達ランク(整数)")]
    public int ArrivalRankInt { get; set; }
    [JsonProperty("次ランクまでの勝利数")]
    public int RestNum { get; set; }
    [JsonProperty("図鑑完成率")]
    public int EncycCompletion { get; set; }


    public static void SetSaveData(int winningStreak, string arrivalRankStr, int arrivalRankInt, int restNum, int encycCompletion)
    {
        var data = new SaveData()
        {
            WinningStreak = winningStreak,
            ArrivalRankStr = arrivalRankStr,
            ArrivalRankInt = arrivalRankInt,
            RestNum = restNum,
            EncycCompletion = encycCompletion,
        };

        File.WriteAllText(Application.persistentDataPath + "/SaveData.json", JsonConvert.SerializeObject(data, Formatting.Indented));
    }
}