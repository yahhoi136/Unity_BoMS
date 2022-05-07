using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageController: MonoBehaviour
{
    // 自分のweaponに渡す用のatk。
    [NotEditable] public float atk;

    // 以下被ダメ処理用。
    float hp;
    [NotEditable] public float damage;
    [NotEditable] public bool isTakeDamage = false;

    public void takeDamage()
    {
        isTakeDamage = true;
    }

    public float hpDecrease(float hp)
    {
        this.hp = hp;
        this.hp -= damage;
        return this.hp;
    }



}
