using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// DontDestroyOnloadで、シーンを跨いで値を保存するだけの存在。これオブジェクトにしなくてもJSONとかに保存するんじゃね？普通。
// でも同じPreparationSceneを使うなら、それが初挑戦かリトライかを判断するために、シーン間でデータ渡す必要はある。オブジェクトをDontDestroyで残したりして。
public class DataForRetry : MonoBehaviour
{

    #region 変数とプロパティ
    // 値確認用SerializeField、後で消す。
    [SerializeField] public List<CharacterStatus> _enemyListForRe;
    [SerializeField] public List<Vector3> _pointListForRe;
    [SerializeField] public List<int> _numOfSpawnForRe;

    public List<CharacterStatus> EnemyListForRe { get { return _enemyListForRe; } set { _enemyListForRe = value; } }
    public List<Vector3> PointListForRe { get { return _pointListForRe; } set { _pointListForRe = value; } }
    public List<int> NumOfSpawnForRe { get { return _numOfSpawnForRe; } set { _numOfSpawnForRe = value; } }
    #endregion

}
