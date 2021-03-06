using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

#region  キャラクターコントローラー説明
/*///

・キャラクターのコンポーネント
 全てのキャラクターは各キャラクターController、StatusController、DamageControllerをコンポーネントとして有する。
 キャラクターControllerで各キャラクターの挙動を管理し、StatusControllerで現在のステータスを管理し、DamageControllerで被ダメージと与ダメージを管理する。
 これにより、キャラクターControllerの名前が異なっていても、StatusControllerの名前で一貫して、外部からの現在ステータスの取得と変更が可能であり、
 DamageControllerの名前で一貫して、攻撃してくる異なる相手のWeapon毎にダメージ処理を行える。

・ターゲット管理
 ターゲットはBattleControllerが管理しており、全てのtargetCharacters[]の呼び出し元になっている。

・BattleSceneでのキャラクターステータス管理
 初期ステータスは各characterStatusDataか、プレイヤーの場合PreparationSceneで設定したPlayerPreparationが呼び出し元になっている。
 現在ステータスはStatusControllerが呼び出し元になっている。

・ダメージ管理
 全てのキャラクターは子オブジェクトであるWeaponオブジェクトで攻撃する。Weaponオブジェクトの名前は「weapon:〇〇」となっている。
 Weaponが相手キャラに当たる　→　当てられたキャラのDamageControllerに、WeaponのAtkと当たったことを代入。
 ReLoadHp()でDamageControllerにHpを代入。DamageControllerの中で与えられたHpとAtkを計算してReLoadHp()にreturnする。

/*///
#endregion


public class WizardPlayerController : MonoBehaviour, ICharacter, IAttackable
{

	// インスペクタで接続
	[SerializeField] Animator animator;
	[SerializeField] CharacterController characterController;
	[SerializeField] GameObject myFireBall;
	[SerializeField] StatusController statusController;
	[SerializeField] DamageController damageController;
	[SerializeField] Canvas hpCanvas;
	[SerializeField] Slider hpSlider;

	[SerializeField, NotEditable] PlayerPreparation playerPreparation;
	BattleController battleController;
	GameObject[] targetCharacters;
	bool gottenBC = false;

	int speedParamHash = Animator.StringToHash("Speed");
	int directionParamHash = Animator.StringToHash("Direction");
	int attackParamHash = Animator.StringToHash("AttackStart");
	int atkRateParamHash = Animator.StringToHash("AtkRate");
	int dieParamHash = Animator.StringToHash("IsDead");


	void Start()
	{
		// TitleSceneでは何もしない。
		if (SceneManager.GetActiveScene().name == "TitleScene") return;

		// prefabに参照させる。相手が親子関係でなくprefabでも無いのでインスペクタ接続できない
		playerPreparation = GameObject.FindGameObjectWithTag("PlayerPreparation").GetComponent<PlayerPreparation>();
	}


	void Update()
	{
		// TitleSceneでは何もしない。
		if (SceneManager.GetActiveScene().name == "TitleScene") return;

		// PreparationSceneでは素振りのみ可能。
		if (SceneManager.GetActiveScene().name == "PreparationScene")
		{
			// 各初期ステータスを現在のステータスに代入
			statusController.dataName = playerPreparation.PlayerName;
			statusController.hp = playerPreparation.PlayerHp;
			statusController.atk = playerPreparation.PlayerAtk;
			statusController.atkRate = playerPreparation.PlayerAtkRate;
			statusController.spd = playerPreparation.PlayerSpd;

			DontDestroyOnLoad(gameObject);
			Attack();
			return;
		}

		// BattleSceneに移動した際に一度だけbattleControllerを取得、DontDestroyOnLoadも解除。
		if (SceneManager.GetActiveScene().name == "BattleScene" && !gottenBC)
		{
			battleController = GameObject.FindGameObjectWithTag("BattleController").GetComponent<BattleController>();
			SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());

			// hpに応じてHpバーの長さを調節
			hpSlider.GetComponent<RectTransform>().sizeDelta = new Vector2(2.5f * playerPreparation.PlayerHp, 20f);
			gottenBC = true;
		}

		hpCanvas.enabled = true;
		targetCharacters = battleController.Enemies;

		// Enemy陣営のキャラが0になった時に操作を止めさせる
		if (targetCharacters.Length == 0 && SceneManager.GetActiveScene().name == "BattleScene")
		{
			animator.SetFloat(speedParamHash, 0);
			return;
		}

		Move();
		Attack();
		ReLoadHp();
	}


	///// 移動処理
	///
	public void Move()
	{
		// 入力値を代入
		float v = CrossPlatformInputManager.GetAxis("Vertical");
		float h = CrossPlatformInputManager.GetAxis("Horizontal");
		Vector3 velocity;

		// 前進後退
		velocity = new Vector3(0, 0, v);
		velocity = transform.TransformDirection(velocity);
		if (v > 0)
		{
			velocity *= statusController.spd;
		}
		else if (v < 0)
		{
			velocity *= (statusController.spd * 0.5f);
		}

		// 重力
		velocity.y -= playerPreparation.PlayerGravity * Time.deltaTime;

		characterController.Move(velocity * Time.deltaTime);

		// 回転
		transform.Rotate(0, h * statusController.spd * 0.5f, 0);

		animator.SetFloat(speedParamHash, v);
		animator.SetFloat(directionParamHash, h);
	}


	///// 攻撃処理
	///
	public void Attack()
	{
		// 現在ステータスの代入
		damageController.atk = statusController.atk;
		animator.SetFloat(atkRateParamHash, playerPreparation.PlayerAtkRate);

		// 入力でアニメーション起動
		if (CrossPlatformInputManager.GetButton("Attack")) { animator.SetTrigger(attackParamHash); }
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
		hpSlider.value = statusController.hp / playerPreparation.PlayerHp;


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
