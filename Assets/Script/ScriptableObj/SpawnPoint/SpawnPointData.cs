using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ファイル名、メニュー表示名、表示順(降順)を指定
[CreateAssetMenu(
  fileName = "SpawnPointData",
  menuName = "ScriptableObject/SpawnPointData",
  order = 2)
]


public class SpawnPointData : ScriptableObject
{
    public List<SpawnPoint> SpawnPointList = new List<SpawnPoint>();
}


// 適応されないものは0かNoneに設定する。例えばPlayerSpriteはPlayerにしか適応されないので他のEnemyとかHomeとかではNone。
[System.Serializable]
public class SpawnPoint
{
    // 一番上のstringはリストの名前になるので常に設定。 
    [SerializeField] private string _name;
    [SerializeField] List<Vector3> _point = new List<Vector3>();

    // z=0が境界線。z<0が自陣。z>0が敵陣。
    public string Name { get { return _name; } }
    public List<Vector3> Point { get{ return _point; } }
}
