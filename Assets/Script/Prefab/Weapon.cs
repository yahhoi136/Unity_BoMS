using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 全てのキャラクターは子オブジェクトのWeaponオブジェクト(Tag:Weapon)によって攻撃する。
// 
public class Weapon : MonoBehaviour
{
    [SerializeField] DamageController myDamageController;

    public float atk;
    string mySide;
    string opponentSide;


    private void Start()
    {
        // 一番上の親のタグでHomeかEnemyか判別
        mySide = transform.root.gameObject.tag;
        // 相手陣営の設定
        opponentSide = mySide == "Home" ? "Enemy" : "Home";
    }


    void Update()
    {
        // PreparationSceneでatkを設定
        if (SceneManager.GetActiveScene().name == "PreparationScene")
        {
            // ステータスの代入
            atk = myDamageController.atk;
        }
    }


    // 与ダメ処理。フレンドリーファイアなし。
    void OnTriggerEnter(Collider other)
    {
        // 相手陣営のキャラクターに当たった時
        if (other.transform.root.gameObject.CompareTag(opponentSide))
        {
            // 当たった武器のAtk分相手のdamageControllerを介して相手のHP減らす。
            DamageController targetDamageController = other.transform.root.gameObject.GetComponent<DamageController>();
            targetDamageController.damage = atk;
            targetDamageController.takeDamage();
        }
    }
}


