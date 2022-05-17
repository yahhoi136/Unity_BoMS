using UnityEngine;

// 全てのキャラクターはWeaponオブジェクトによって攻撃する。Weaponオブジェクトの名前は「weapon:〇〇」になっている。
// 近距離武器はShortRangeWeaponに、遠距離武器はLongRangeWeaponに管理される。
// 
public class LongRangeWeapon : MonoBehaviour
{
    [SerializeField] LongRangeWeaponData LRWdata;
    [SerializeField] int weaponNum;
    [SerializeField] Rigidbody myRb;
    [NotEditable] public float atk;
    DamageController myDamageController;
    LRW LRW;
    string mySide;
    string opponentSide;


    private void Start()
    {
        LRW = LRWdata.LRWList[weaponNum];
        myDamageController = transform.root.gameObject.GetComponent<DamageController>();

        // 一番上の親のタグでHomeかEnemyか判別
        mySide = transform.root.gameObject.tag;
        // 相手陣営の設定
        opponentSide = mySide == "Home" ? "Enemy" : "Home";

        // 親を外して独立させる
        transform.parent = null;
    }


    void Update()
    {
        // キャラの攻撃力から武器の攻撃力を設定
        atk = myDamageController.atk;
        myRb.velocity = transform.TransformDirection(new Vector3(0, -LRW.Gravity, LRW.Spd));
    }


    // 与ダメ処理。フレンドリーファイアなし。
    void OnTriggerEnter(Collider other)
    {
        // 相手陣営のキャラクターに当たった時
        if (other.transform.root.gameObject.CompareTag(opponentSide))
        {
            // 武器のAtk分相手のdamageControllerを介して相手のHP減らす。
            DamageController targetDamageController = other.transform.root.gameObject.GetComponent<DamageController>();
            targetDamageController.damage = atk;
            targetDamageController.takeDamage();
        }

        // 衝突するとエフェクト発生して消える
        var effect = Instantiate(LRW.Effect, transform.position, Quaternion.identity);
        Destroy(gameObject);
        Destroy(effect, LRW.DisappearTime);
    }
}


