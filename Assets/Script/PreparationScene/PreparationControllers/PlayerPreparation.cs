using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region 説明
/*///

・リトライ用のデータ管理
 初挑戦時とリトライ時での動作を変える。リトライ時か否か(isRetried)はPreparationController、リトライ用のデータはDataForRetryが全ての呼び出し元になっている。

・コスト管理
 「①Difficulty設定→　②EnemyCostLimit設定→　③HomeCostLimit設定→　④各必要コストからTotalHomeCostの計算」の順に行われる。
 ①〜③はEnemySpawnControllerで管理。④は各ボタンで管理。全てのHomeCostLimit,TotalHomeCostの呼び出し元はPlayerPreparationになっている。

・プレイヤーステータス管理
 プレイヤーステータス管理は「①PlayerNum設定→　②MyStatusを設定→　③PlayerLevelingDataと各種初期ステータスを設定→
 ④各Up/Downボタンタップ時にステータス変動」の順に行われる。①〜③はPlayerChangeボタンで管理。④は各種ボタンで管理している。
 全てのPlayerNum,MyStatus,PlayerLevelingData,各Playerステータスの呼び出し元はPlayerPreparationになっている。

・キャラ配置管理
「」の順に行われる。

・読み込み優先
 ScriptExcutionOrderで読み込みを、PreparationController →　EnemySpawnController →　PlayerPreparation or AllocationControllerの順で優先

/*///
#endregion


public class PlayerPreparation : MonoBehaviour
{
    #region 変数とプロパティ
    [SerializeField] CharacterStatusData playerStatusData;

    // 数値確認
    [SerializeField, NotEditable] PlayerLevelingData _playerLevelingData;
    [SerializeField, NotEditable] CharacterStatus _myStatus;
    [SerializeField, NotEditable] int _homeCostLimit, _totalHomeCost, _playerNum, _hpLv, _atkLv, _spdLv, _atkRateLv;
    [SerializeField, NotEditable] float _playerHp, _playerAtk, _playerAtkRate, _playerSpd;

    public PlayerLevelingData PlayerLevelingData { get{ return _playerLevelingData; } set { _playerLevelingData = value; } }
    public CharacterStatus MyStatus { get{ return _myStatus; } set { _myStatus = value; } }
    public int HomeCostLimit { get { return _homeCostLimit; } set { _homeCostLimit = value; } }
    public int TotalHomeCost { get { return _totalHomeCost; } set { _totalHomeCost = value; } }
    public int PlayerNum { get { return _playerNum; }   set { _playerNum = value; } }

    public int HpLv { get { return _hpLv; } set { _hpLv = value; } }
    public int AtkLv { get { return _atkLv; } set { _atkLv= value; } }
    public int SpdLv { get { return _spdLv; } set { _spdLv = value; } }
    public int AtkRateLv { get { return _atkRateLv; } set { _atkRateLv = value; } }

    public float PlayerHp { get { return _playerHp; } set { _playerHp = value; } }
    public float PlayerAtk { get { return _playerAtk; } set { _playerAtk = value; } }
    public float PlayerAtkRate { get { return _playerAtkRate; } set { _playerAtkRate = value; } }
    public float PlayerSpd { get { return _playerSpd; } set { _playerSpd = value; } }
    #endregion


    void Start()
    {

        HpLv = 1; AtkLv = 1; SpdLv = 1; AtkRateLv = 1;
        // きつね犬を初期値に。
        MyStatus = playerStatusData.CharacterStatusList[0];
        PlayerLevelingData = MyStatus.PlayerLevelingData;
        PlayerHp = MyStatus.Hp;
        PlayerAtk = MyStatus.Atk;
        PlayerAtkRate = MyStatus.AtkRate;
        PlayerSpd = MyStatus.Spd;
        
        // 初期値できつね犬prefab出します。
        // prefabからここの参照を取らせます
        // playerChangeで変更する度に今でてるplayerPrefabをdestroyして次のPlayerPrefabを出します。
        // prefabの値を設定します。
    }

}

