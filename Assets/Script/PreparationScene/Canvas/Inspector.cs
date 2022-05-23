using UnityEngine;
using UnityEngine.UI;


public class Inspector : MonoBehaviour
{

    [SerializeField] Text nameText;
    [SerializeField] Text otherText;
    [SerializeField] PlayerPreparation playerPreparation;

    StatusController pickedCharaStatus;

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
                pickedCharaStatus = hit.collider.transform.root.gameObject.GetComponent<StatusController>();
                reloadInspector();
            }
        }
    }

    void reloadInspector()
    {
        nameText.text = $"ステータス\n【{pickedCharaStatus.dataName}】";
        otherText.text = $"HP                  {pickedCharaStatus.hp}\nATK                  {pickedCharaStatus.atk}\nATK RATE     {pickedCharaStatus.atkRate}/s\nSPEED          {pickedCharaStatus.spd}m/s";
    }
}
