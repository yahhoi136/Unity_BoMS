using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Inspector : MonoBehaviour
{

    [SerializeField] Text myText;
    [SerializeField] PlayerPreparation playerPreparation;


    void Start()
    {
        // ここの設定から続き。${}してね。
        myText.text = "\n【Player】\n\nHP                  100\nATK                  20\nATK RATE     1.5s\nSPEED          4m / s";
    }


    void Update()
    {
        
    }

    
    // クリックした時に、Ray衝突が、FloorでもWallでも無い時→　つまりキャラの時、そのキャラのStatusをインスペクタに表示
    // キャラクターステータスの方にscriptableObj接続して、その中でStartでステータス設定しないとな、。
    // PlayerのLayerの時だけ、PlayerStatusを参照する。


    // startはplayer
    // クリックされたらそのキャラクター
    // Allocationボタン押されたら、そのselectされたボタンのキャラクター
    // playerStatusUpボタン押されたらplayer(てかキャラクターでもAllocationボタンでも無いところをクリックされた時で多分OK)
}
