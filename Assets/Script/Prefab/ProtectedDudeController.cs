using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// このキャラクターは攻撃できない。
// 
public class ProtectedDudeController : MonoBehaviour, ICharacter
{

    // インスペクタで接続
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody rb;
    [SerializeField] MeshCollider myWeaponMc;
    [SerializeField] CharacterStatusData characterStatusData;
    // 他で実装
    [SerializeField] WhoisClosestTarget whoisClosestTarget;
    [SerializeField] StatusController statusController;
    [SerializeField] DamageController damageController;

    BattleController battleController;
    CharacterStatus myStatus;
    GameObject[] targetCharacters;
    GameObject closestTarget;
    float distance;
    bool gottenBC = false;
    int moveParamHash = Animator.StringToHash("IsMoving");
    int dieParamHash = Animator.StringToHash("IsDead");


    void Start()
    {
        // 各初期ステータスを現在のステータスに代入
        myStatus = characterStatusData.CharacterStatusList[1];
        statusController.dataName = myStatus.Name;
        statusController.cost = myStatus.Cost;
        statusController.hp = myStatus.Hp;
        statusController.spd = myStatus.Spd;
    }


    void Update()
    {
        // PreparationSceneでは動かさない
        if (SceneManager.GetActiveScene().name == "PreparationScene")
        {
            DontDestroyOnLoad(gameObject);
            return;
        }

        // BattleSceneに移動した際に一度だけbattleControllerを取得、DontDestroyOnLoadも解除。
        if (SceneManager.GetActiveScene().name == "BattleScene" && !gottenBC)
        {
            battleController = GameObject.FindGameObjectWithTag("BattleController").GetComponent<BattleController>();
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
            gottenBC = true;
        }

        targetCharacters = myStatus.MySide == "Enemy" ? battleController.Homes : battleController.Enemies;

        // 相手陣営のキャラが0になった時に動作を止める
        if (targetCharacters.Length == 0)
        {
            animator.SetBool(moveParamHash, false);
            rb.velocity = Vector3.zero;
            return;
        }
        else
        {
            // 最短距離にある相手陣営オブジェクトとその距離の取得
            closestTarget = whoisClosestTarget.whoisClosest(targetCharacters);
            distance = (closestTarget.transform.position - transform.position).sqrMagnitude;

            Move();
            ReLoadHp();
        }
    }


    ///// 移動処理
    ///
    public void Move()
    {
        // 最短距離にある相手陣営オブジェクトに攻撃範囲まで接近
        if (myStatus.AtkRange < distance)
        {
            transform.LookAt(closestTarget.transform.position);
            rb.velocity = new Vector3(0, -myStatus.Gravity, statusController.spd);
            rb.velocity = transform.TransformDirection(rb.velocity);
            animator.SetBool(moveParamHash, true);
        }
        else
        {
            transform.LookAt(closestTarget.transform.position);
            rb.velocity = new Vector3(0, -myStatus.Gravity, 0);
            animator.SetBool(moveParamHash, false);
        }
    }


    ///// Hp管理
    /// 
    public void ReLoadHp()
    {
        // 被ダメ処理与ダメ処理は各Weaponオブジェクトに
        if (damageController.isTakeDamage)
        {
            statusController.hp = damageController.hpDecrease(statusController.hp);
            damageController.isTakeDamage = false;

            if (statusController.hp <= 0)  // 死亡時
            {
                animator.SetTrigger(dieParamHash);
                this.enabled = false;
            }
        }
    }

    // コールバックで死亡アニメーション後にオブジェクト削除＆ターゲットリスト更新。
    public void characterDead()
    {
        Destroy(gameObject);
    }
}