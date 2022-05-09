using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Inspector : MonoBehaviour
{

    [SerializeField] Text nameText;
    [SerializeField] Text otherText;
    [SerializeField] PlayerPreparation playerPreparation;

    StatusController pickedCharaStatus;
    bool isCharaPicked;
    bool isNeutral = true;


    void Update()
    {

        // クリックした時
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int playerLayerMask = 1 << 8;
            int homeCharaLayerMask = 1 << 10;
            int enemyCharaLayerMask = 1 << 12;


            // Rayがキャラクターに衝突した時、そのキャラのStatusをインスペクタに表示
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, playerLayerMask) || Physics.Raycast(ray, out hit, Mathf.Infinity, homeCharaLayerMask) || Physics.Raycast(ray, out hit, Mathf.Infinity, enemyCharaLayerMask))
            {
                isCharaPicked = true;
                isNeutral = false;
                pickedCharaStatus = hit.collider.transform.root.gameObject.GetComponent<StatusController>();
            }

            // キャラクター以外の虚空がクリックされた時、PlayerのStatusをインスペクタに表示
            else
            {
                isCharaPicked = false;
                isNeutral = true;
            }

        }


        if (isCharaPicked)
        {
            nameText.text = $"【{pickedCharaStatus.dataName}】";
            otherText.text = $"HP                  {pickedCharaStatus.hp}\nATK                  {pickedCharaStatus.atk}\nATK RATE     {pickedCharaStatus.atkRate}/s\nSPEED          {pickedCharaStatus.spd}m/s";
        }

        if (isNeutral) 
        {
            nameText.text = $"【{playerPreparation.PlayerName}】";
            otherText.text = $"HP                  {playerPreparation.PlayerHp}\nATK                  {playerPreparation.PlayerAtk}\nATK RATE     {playerPreparation.PlayerAtkRate}/s\nSPEED          {playerPreparation.PlayerSpd}m/s";
        }
    }

}
