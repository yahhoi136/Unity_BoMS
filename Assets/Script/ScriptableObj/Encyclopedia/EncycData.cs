using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ファイル名、メニュー表示名、表示順(降順)を指定
[CreateAssetMenu(
  fileName = "EncycData",
  menuName = "ScriptableObject/EncycData",
  order = 5)
]


public class EncycData : ScriptableObject
{
    public List<Encyc> EncycList = new List<Encyc>();
}


[System.Serializable]
public class Encyc
{
    // 一番上のstringはリストの名前になるので常に設定。 
    // ステータスを調節する時は、先にCharacterStatusを設定した後に、こっちを手動で適応させる。CharacterStatusDataとEncycDataは同期していない。
    // Explanationは一つ一つ実際のテキストを入れてみて、句読点や括弧などの変な改行が行われないように、適宜スペースなどを入れて調節する。
    [SerializeField] string _name;
    [SerializeField] GameObject _demoPrefab;
    [SerializeField] string _rank, _cost, _hp, _atk, _atkRate, _spd;
    [SerializeField, TextArea(2, 5)] string _explanation;

    public string Name { get { return _name; } }
    public GameObject DemoPrefab { get{ return _demoPrefab; } }
    public string Rank { get{ return _rank; } }
    public string Cost { get{ return _cost; } }
    public string Hp { get{ return _hp; } }
    public string Atk { get{ return _atk; } }
    public string AtkRate { get{ return _atkRate; } }
    public string Spd { get{ return _spd; } }
    public string Explanation { get { return _explanation; } }

}
