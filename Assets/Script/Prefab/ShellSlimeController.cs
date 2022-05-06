using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// SpawnするEnemyのPrefabの大きさは x×z = 4×4以内の大きさであればなんでもOK。
public class ShellSlimeController : MonoBehaviour, ICharacter, IAttackable
{

    // インスペクタで接続
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody rb;
    [SerializeField] MeshCollider myWeaponMc;
    [SerializeField] CharacterStatusData characterStatusData;
    // 他で実装
    [SerializeField] WhoisClosestTarget whoisClosestTarget;
    [SerializeField] DamageController damageController;

    BattleController battleController;


    float hp, spd;
    CharacterStatus myStatus;
    GameObject[] targetCharacters;
    GameObject closestTarget;
    float distance;
    bool gottenBC = false;
    int inAtkRangeParamHash = Animator.StringToHash("InAtkRange");
    int atkRateParamHash = Animator.StringToHash("AtkRate");
    int moveParamHash = Animator.StringToHash("IsMoving");
    int dieParamHash = Animator.StringToHash("IsDead");
    int baseAttackParamHash = Animator.StringToHash("Base Layer.Attack");


    void Start()
    {

        // ステータス設定
        myStatus = characterStatusData.CharacterStatusList[1];
        hp = myStatus.Hp;
        damageController.atk = myStatus.Atk;
        spd = myStatus.Spd;
        animator.SetFloat(atkRateParamHash, myStatus.AtkRate);
    }


    void Update()
    {
        // TitleSceneでは何も出来ない。
        if (SceneManager.GetActiveScene().name == "TitleScene") return;

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
            animator.SetBool(inAtkRangeParamHash, false);
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
            Attack();
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
            rb.velocity = new Vector3(0, -myStatus.Gravity, spd);
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


 ///// 攻撃処理
 /// 
    public void Attack()
    {
        AnimatorStateInfo state;

        // 最短距離にある相手陣営オブジェクトが攻撃範囲内だと攻撃アクション行う
        animator.SetBool(inAtkRangeParamHash, distance < myStatus.AtkRange ? true : false);

        // 攻撃アニメーション中のみ武器の判定を有効に
        state = animator.GetCurrentAnimatorStateInfo(0);
        myWeaponMc.enabled = state.fullPathHash == baseAttackParamHash ? true : false;
    }


 ///// Hp管理
 /// 
    public void ReLoadHp()
    {
        // 被ダメ処理与ダメ処理は各Weaponオブジェクトに
        if (damageController.isTakeDamage)
        {
            hp = damageController.hpDecrease(hp);
            damageController.isTakeDamage = false;

            if (hp <= 0)  // 死亡時
            {
                animator.SetTrigger(dieParamHash);
                myWeaponMc.enabled = false;  // これないと死亡アニメーション中に攻撃してくる
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