using UnityEngine;

// 全てのキャラクターはWeaponオブジェクトによって攻撃する。Weaponオブジェクトの名前は「weapon:〇〇」になっている。
// 近距離武器はShortRangeWeaponに、遠距離武器はLongRangeWeaponに管理される。
// 
public class ShortRangeWeapon : MonoBehaviour
{
    DamageController myDamageController;

    [NotEditable] public float atk;
    string mySide;
    string opponentSide;


    private void Start()
    {
        myDamageController = transform.root.gameObject.GetComponent<DamageController>();

        // 一番上の親のタグでHomeかEnemyか判別
        mySide = transform.root.gameObject.tag;
        // 相手陣営の設定
        opponentSide = mySide == "Home" ? "Enemy" : "Home";
    }


    // 与ダメ処理。フレンドリーファイアなし。
    void OnTriggerEnter(Collider other)
    {
        // キャラの攻撃力から武器の攻撃力を設定
        atk = myDamageController.atk;

        // 相手陣営のキャラクターに当たった時
        if (other.transform.root.gameObject.CompareTag(opponentSide))
        {
            // 武器のAtk分相手のdamageControllerを介して相手のHP減らす。
            DamageController targetDamageController = other.transform.root.gameObject.GetComponent<DamageController>();
            targetDamageController.damage = atk;
            targetDamageController.takeDamage();
        }
    }
}


