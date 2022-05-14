using UnityEngine;

#region PreparatioinControllers説明
/*///

・リトライ用のデータ管理
 初挑戦時とリトライ時での動作を変える。リトライ時か否か(isRetried)はPreparationController、リトライ用のデータはDataForRetryが全ての呼び出し元になっている。

・コストと敵キャラ配置管理
 「①Difficulty設定→　②EnemyCostLimit設定→　③HomeCostLimit設定→　④各必要コストからTotalHomeCostの計算」の順に行われる。
 ①〜③はEnemySpawnControllerで管理。④は各ボタンで管理。全てのHomeCostLimit,TotalHomeCostの呼び出し元はPlayerPreparationになっている。
 ちなみにDifficultyDataのMinRankとMaxRankの差は0〜1程度にしておかないとsetNumOfSpawn()の中のwhile文が永遠に終わらなくてバグる。

・PreparationSceneでのプレイヤーステータス管理(戦闘開始時初期プレイヤーステータスの設定)
 「①PlayerNum設定→　②MyStatusを設定→　③PlayerLevelingDataと各種初期ステータスを設定→④各Up/Downボタンタップ時にステータス変動」の順に行われる。
 ①〜③はPlayerChangeボタンで管理。④は各種ボタンで管理している。全てのPlayerNum,MyStatus,PlayerLevelingData,Preparationでの各Playerステータス,
 の呼び出し元はPlayerPreparationになっている。
 ちなみに、AtkRateとは、1秒間の攻撃回数になっている。全てのキャラは1秒に1回だけ攻撃アニメーションをするように(1度に2回攻撃するものは2秒に1回だけ)、
 Animatorで調節しており、その倍数のMultiplierとしてAtkrateで攻撃速度を設定できる。

・味方キャラ配置管理
「①新規キャラの配置/配置取り消し→　②既存キャラの移動/削除」の順に行われる。
 ①は各AllocationボタンとAllocationDeleteによって管理されており、②はAllocationControllerとAllocationDeleteによって管理されている。
 
・読み込み優先
 ScriptExcutionOrderで読み込みを、PreparationController →　EnemySpawnController →　PlayerPreparation or AllocationControllerの順で優先

/*///
#endregion


public class PlayerPreparation : MonoBehaviour
{
    #region 変数とプロパティ
    // 数値確認
    [SerializeField, NotEditable] PlayerLevelingData _playerLevelingData;
    [SerializeField, NotEditable] CharacterStatus _myStatus;
    [SerializeField, NotEditable] int _homeCostLimit, _totalHomeCost, _playerNum, _hpLv, _atkLv, _spdLv, _atkRateLv;
    [SerializeField, NotEditable] string _playerName;
    [SerializeField, NotEditable] float _playerHp, _playerAtk, _playerAtkRate, _playerSpd, _playerGravity;

    public PlayerLevelingData PlayerLevelingData { get{ return _playerLevelingData; } set { _playerLevelingData = value; } }
    public CharacterStatus MyStatus { get{ return _myStatus; } set { _myStatus = value; } }
    public int HomeCostLimit { get { return _homeCostLimit; } set { _homeCostLimit = value; } }
    public int TotalHomeCost { get { return _totalHomeCost; } set { _totalHomeCost = value; } }
    public int PlayerNum { get { return _playerNum; }   set { _playerNum = value; } }

    public int HpLv { get { return _hpLv; } set { _hpLv = value; } }
    public int AtkLv { get { return _atkLv; } set { _atkLv= value; } }
    public int SpdLv { get { return _spdLv; } set { _spdLv = value; } }
    public int AtkRateLv { get { return _atkRateLv; } set { _atkRateLv = value; } }

    public string PlayerName { get { return _playerName; } set { _playerName = value; } }
    public float PlayerHp { get { return _playerHp; } set { _playerHp = value; } }
    public float PlayerAtk { get { return _playerAtk; } set { _playerAtk = value; } }
    public float PlayerAtkRate { get { return _playerAtkRate; } set { _playerAtkRate = value; } }
    public float PlayerSpd { get { return _playerSpd; } set { _playerSpd = value; } }
    public float PlayerGravity { get{ return _playerGravity; } set { _playerGravity = value; } }
    #endregion

    public delegate void Reset();
    public Reset resetLv;
    public Reset resetText;
}

