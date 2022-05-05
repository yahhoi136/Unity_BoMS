using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ファイル名、メニュー表示名、表示順(降順)を指定
[CreateAssetMenu(
  fileName = "PlayerLevelingData",
  menuName = "ScriptableObject/PlayerLevelingData",
  order = 1)
]


public class PlayerLevelingData : ScriptableObject
{
    public List<PlayerLeveling> playerLevelingList = new List<PlayerLeveling>();
}


// 適応されないものは0かNoneに設定する。例えばPlayerSpriteはPlayerにしか適応されないので他のEnemyとかHomeとかではNone。
[System.Serializable]
public class PlayerLeveling
{
    // 一番上のstringはリストの名前になるので常に設定。 
    [SerializeField] private string _name;
    [SerializeField] private int _maxLv;
    [SerializeField] List<int> _cost = new List<int>();
    [SerializeField] List<float> _incre = new List<float>();

    public string Name { get { return _name; } }
    public int MaxLv { get { return _maxLv; } }

    // レベル間コスト。index0から順にLv１→２になるときのコスト
    public List<int> Cost { get { return _cost; } }

    // レベル間増分(increment)。index0から順にLv１→２になるときの増分
    public List<float> Incre { get { return _incre; } }

}
