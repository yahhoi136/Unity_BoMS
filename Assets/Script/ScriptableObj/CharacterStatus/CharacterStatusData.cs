using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ファイル名、メニュー表示名、表示順(降順)を指定
[CreateAssetMenu(
  fileName = "CharacterStatusData",
  menuName = "ScriptableObject/CharacterStatusData",
  order = 0)
]


public class CharacterStatusData : ScriptableObject
{
    public List<CharacterStatus> CharacterStatusList = new List<CharacterStatus>();
}


// 適応されないものは0かNoneに設定する。例えばPlayerSpriteはPlayerにしか適応されないので他のEnemyとかHomeとかではNone。
[System.Serializable]
public class CharacterStatus
{
    // 一番上のstringはリストの名前になるので常に設定。 
    [SerializeField] private string _name, _mySide;
    [SerializeField] private int _enemyRnak, _cost;
    [SerializeField] private float _hp, _atk, _atkRange, _atkRate, _spd, _gravity;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Sprite _playerSprite;
    [SerializeField] private PlayerLevelingData _playerLevelingData;

    public string Name { get { return _name; } }
    public string MySide { get { return _mySide; } }
    public int EnemyRank { get{ return _enemyRnak; } }
    public int Cost { get { return _cost; } }
    public float Hp { get{ return _hp; } }
    public float Atk { get { return _atk; } }
    public float AtkRange { get{ return _atkRange; } }
    public float AtkRate { get { return _atkRate; } }
    public float Spd { get{ return _spd; } }
    public float Gravity { get { return _gravity; }}
    public GameObject Prefab { get { return _prefab; } }
    public Sprite PlayerSprite { get{ return _playerSprite; } }
    public PlayerLevelingData PlayerLevelingData { get { return _playerLevelingData; } }


}
