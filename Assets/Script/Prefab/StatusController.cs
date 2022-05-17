using UnityEngine;

public class StatusController : MonoBehaviour
{

    // gravityはキャラの衝突時の挙動管理のためのもので、atkRangeはそれぞれのキャラの攻撃アニメーションとリンクしてるので、バトル中の強化弱体による変更不可。
    [NotEditable] public string dataName;
    [NotEditable] public int cost;
    [NotEditable] public float hp, atk, atkRate, spd;

}
