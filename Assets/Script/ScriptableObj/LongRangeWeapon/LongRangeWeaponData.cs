using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ファイル名、メニュー表示名、表示順(降順)を指定
[CreateAssetMenu(
  fileName = "LongRangeWeaponData",
  menuName = "ScriptableObject/LongRangeWeaponData",
  order = 4)
]


public class LongRangeWeaponData : ScriptableObject
{
    public List<LRW> LRWList = new List<LRW>();
}


// 適応されないものは0かNoneに設定する。例えばPlayerSpriteはPlayerにしか適応されないので他のEnemyとかHomeとかではNone。
[System.Serializable]
public class LRW
{
    // 一番上のstringはリストの名前になるので常に設定。 
    [SerializeField] private string _name;
    [SerializeField] GameObject _effect;
    [SerializeField] float _gravity, _spd, _disappearTime;

    public string Name { get { return _name; } }
    public GameObject Effect { get{ return _effect; } }
    public float Gravity { get{ return _gravity; } }
    public float Spd { get{ return _spd; } }
    public float DisappearTime { get{ return _disappearTime; } }


}
