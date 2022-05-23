using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// SpawnするEnemyのPrefabの大きさは X×Z = 4×4以内の大きさであればなんでもOK。
public class DragonController : MonoBehaviour, ICharacter, IAttackable
{

    // インスペクタで接続
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject myFireBall;
    [SerializeField] CharacterStatusData characterStatusData;
    [SerializeField] int characterNum;
    [SerializeField] Canvas hpCanvas;
    [SerializeField] Slider hpSlider;
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

    int inAtkRangeParamHash = Animator.StringToHash("InAtkRange");
    int atkRateParamHash = Animator.StringToHash("AtkRate");
    int moveParamHash = Animator.StringToHash("IsMoving");
    int dieParamHash = Animator.StringToHash("IsDead");


    void Start()
    {
        // 各初期ステータスを現在のステータスに代入
        myStatus = characterStatusData.CharacterStatusList[characterNum];
        statusController.dataName = myStatus.Name;
        statusController.cost = myStatus.Cost;
        statusController.hp = myStatus.Hp;
        statusController.atk = myStatus.Atk;
        statusController.atkRate = myStatus.AtkRate;
        statusController.spd = myStatus.Spd;

        // hpに応じてHpバーの長さを調節
        hpSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(2.5f * myStatus.Hp, 20f);
    }


    void Update()
    {
        // BattleScene以外では動かさない
        if (SceneManager.GetActiveScene().name == "TitleScene") return;
        if (SceneManager.GetActiveScene().name == "PreparationScene")
        {
            // 配置時に滑らないように
            rb.constraints = RigidbodyConstraints.FreezeAll;
            DontDestroyOnLoad(gameObject);
            return;
        }

        // BattleSceneに移動した際に一度だけbattleControllerを取得、DontDestroyOnLoadも解除。
        if (SceneManager.GetActiveScene().name == "BattleScene" && !gottenBC)
        {
            battleController = GameObject.FindGameObjectWithTag("BattleController").GetComponent<BattleController>();
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
            gottenBC = true;
        }

        hpCanvas.enabled = true;
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


    ///// 攻撃処理
    /// 
    public void Attack()
    {
        // 現在ステータスの代入
        damageController.atk = statusController.atk;
        animator.SetFloat(atkRateParamHash, statusController.atkRate);

        // 最短距離にある相手陣営オブジェクトが攻撃範囲内だと攻撃アクション行う
        animator.SetBool(inAtkRangeParamHash, distance < myStatus.AtkRange ? true : false);

    }

    // コールバックで攻撃アニメーション中に一度だけ火の玉を生成
    public void attack()
    {
        Instantiate(myFireBall, transform);
    }


    ///// Hp管理
    /// 
    public void ReLoadHp()
    {
        // 常にHpCanvasをMain Cameraに向かせる＆HpSliderの設定
        hpCanvas.transform.rotation = Camera.main.transform.rotation;
        hpSlider.value = statusController.hp / myStatus.Hp;


        // 被ダメ処理与ダメ処理は各Weaponオブジェクトに
        if (damageController.isTakeDamage)
        {
            statusController.hp = damageController.hpDecrease(statusController.hp);
            damageController.isTakeDamage = false;

            if (statusController.hp <= 0)  // 死亡時
            {
                animator.SetTrigger(dieParamHash);
                hpSlider.value = 0;
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