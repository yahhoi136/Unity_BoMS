using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

#region 説明
/*///

・ターゲット管理
 ターゲットはBattleControllerが管理しており、全てのtargetCharacters[]の呼び出し元になっている。

・プレイヤーステータス管理
 ①Hp、AtkRate、Spdのみ各PlayerControllerで管理。AtkRateはアニメーションの再生速度で調節している。
 ②AtkはDamageControllerを介して各Playerの子オブジェクトであるWeaponに渡す。

・ダメージ管理
 全てのキャラクターは子オブジェクトであるWeaponで攻撃する。Atkとダメージ処理は各Weaponが行う。Weaponの名前は「weapon:〇〇」となっている。
 全てのキャラクターはキャラクターControllerとDamageControllerをコンポーネントとして有する。これにより、キャラクターControllerの名前が異なっていても
 DamageControllerを介して、一貫してWeaponとダメージ処理を行える。

/*///
#endregion


public class FoxDogPlayerController : MonoBehaviour, ICharacter, IAttackable
{

	// インスペクタで接続
	[SerializeField] CharacterStatusData characterStatusData;
	[SerializeField] Animator animator;
	[SerializeField] CharacterController character;
	[SerializeField] MeshCollider myWeaponMc;
	[SerializeField] DamageController damageController;

	[SerializeField] PlayerPreparation playerPreparation; // 確認用serializedField後で消す。
	BattleController battleController;
	GameObject[] targetCharacters;
	[SerializeField] float hp, spd, gravity; // 確認用serializedField後で消す。
	bool gottenBC = false;

	int speedParamHash = Animator.StringToHash("Speed");
	int directionParamHash = Animator.StringToHash("Direction");
	int attackParamHash = Animator.StringToHash("AttackStart");
	int atkRateParamHash = Animator.StringToHash("AtkRate");
	int dieParamHash = Animator.StringToHash("IsDead");
	int baseAttackParamHash = Animator.StringToHash("Base Layer.Attack");


	void Start()
    {
		// prefabに参照させる
		playerPreparation = GameObject.FindGameObjectWithTag("PlayerPreparation").GetComponent<PlayerPreparation>();
		gravity = characterStatusData.CharacterStatusList[0].Gravity;
	}


	void Update()
	{
		// PreparationSceneでは素振りのみ可能。
		if (SceneManager.GetActiveScene().name == "PreparationScene")
		{
            // 各種ステータスの代入
            hp = playerPreparation.PlayerHp;
			damageController.atk = playerPreparation.PlayerAtk;
			animator.SetFloat(atkRateParamHash, playerPreparation.PlayerAtkRate);
			spd = playerPreparation.PlayerSpd;

			DontDestroyOnLoad(gameObject);
			Attack();
			return;
		}

		// BattleSceneに移動した際に一度だけbattleControllerを取得、DontDestroyOnLoadも解除。
		if (SceneManager.GetActiveScene().name == "BattleScene" && !gottenBC)
		{
			battleController = GameObject.FindGameObjectWithTag("BattleController").GetComponent<BattleController>();
			SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
			gottenBC = true;
		}

		targetCharacters = battleController.Enemies;

		// Enemy陣営のキャラが0になった時に操作を止めさせる
		if (targetCharacters.Length == 0)
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
			velocity *= spd;
		}
		else if (v < 0)
		{
			velocity *= (spd * 0.5f);
		}

		// 重力
		velocity.y -= gravity * Time.deltaTime;

		character.Move(velocity * Time.deltaTime);

		// 回転
		transform.Rotate(0, h * spd * 0.5f, 0);

		animator.SetFloat(speedParamHash, v);
		animator.SetFloat(directionParamHash, h);
	}


 ///// 攻撃処理
 ///
	public void Attack()
    {
		AnimatorStateInfo state;

		// 入力値を代入
		if (CrossPlatformInputManager.GetButton("Attack"))
        {
			animator.SetTrigger(attackParamHash);

		}

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
