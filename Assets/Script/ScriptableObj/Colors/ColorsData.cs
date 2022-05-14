using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ファイル名、メニュー表示名、表示順(降順)を指定
[CreateAssetMenu(
  fileName = "ColorsData",
  menuName = "ScriptableObject/ColorsData",
  order = 6)
]


public class ColorsData : ScriptableObject
{
    public List<Colors> ColorsList = new List<Colors>();
}


// 適応されないものは0かNoneに設定する。例えばPlayerSpriteはPlayerにしか適応されないので他のEnemyとかHomeとかではNone。
[System.Serializable]
public class Colors
{
    // 一番上のstringはリストの名前になるので常に設定。 
    [SerializeField] string _identifier;
    [SerializeField] Color _color;

    // z=0が境界線。z<0が自陣。z>0が敵陣。
    public string Identifier { get { return _identifier; } }
    public Color Color { get { return _color; } }

}