using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////
////// 最短距離にある相手陣営オブジェクトの取得
///

public class WhoisClosestTarget : MonoBehaviour
{
    public GameObject whoisClosest(GameObject[] targetCharacters)
    {

        Vector3 targetPos;
        Vector3 myPos;
        float distance;
        float preDistance;
        GameObject closestTarget;
        int closestTargetindex = 0;

        // 相手陣営のキャラが一つの時
        if (targetCharacters.Length == 1)　　
        {
            closestTarget = targetCharacters[0];
        }
        else // 相手陣営のキャラが複数の時
        {
            // 0番目のオブジェクトと自分との距離を、前回の距離に設定。
            targetPos = targetCharacters[0].transform.position;
            myPos = transform.position;
            preDistance = (targetPos - myPos).sqrMagnitude;

            // 1番目以降は、前回の距離と比較して小さい方の配列index番号を残す
            for (int i = 1; i < targetCharacters.Length; i++)
            {
                targetPos = targetCharacters[i].transform.position;
                myPos = transform.position;
                distance = (targetPos - myPos).sqrMagnitude;

                {
                    // 前回と今回の距離の比較
                    if (preDistance < distance)
                    {
                        closestTargetindex = i - 1;
                    }
                    else
                    {
                        closestTargetindex = i;
                    }
                }
                preDistance = distance;       // 今回の距離を前回の距離に設定する
            }
            closestTarget = targetCharacters[closestTargetindex];
        }
        return closestTarget;
    }
}
