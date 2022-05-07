using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ファイル名、メニュー表示名、表示順(降順)を指定
[CreateAssetMenu(
  fileName = "DifficultyData",
  menuName = "ScriptableObject/DifficultyData",
  order = 3)
]


public class DifficultyData : ScriptableObject
{
    public List<Difficulty> DifficultyList = new List<Difficulty>();
}


// 適応されないものは0かNoneに設定する。例えばPlayerSpriteはPlayerにしか適応されないので他のEnemyとかHomeとかではNone。
[System.Serializable]
public class Difficulty
{
    // 一番上のstringはリストの名前になるので常に設定。 
    [SerializeField] private string _rank;
    [SerializeField] private int _numOfPhase, _minRank, _maxRank, _minCost, _maxCost;

    // 敵の総コストはMinCost〜MaxCostの間。
    public string Rank { get { return _rank; } }
    public int NumOfPhase { get { return _numOfPhase; } }
    public int MinRank { get{ return _minRank; } }
    public int MaxRank { get { return _maxRank; } }
    public int MinCost { get{ return _minCost; } }
    public int MaxCost { get{ return _maxCost; } }
}
