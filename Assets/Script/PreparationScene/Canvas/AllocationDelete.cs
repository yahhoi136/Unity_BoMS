using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#region 説明
/*///

・AllocationDeleteボタン
 PushイベントでAllocationDeleteボタンを作動させたかったが、PushだとAllocationボタンのSelect状態が解除されるため
 AllocationDeleteボタンにEnterしたかどうかで擬似的に作動させることにしている

/*///
#endregion


public class AllocationDelete : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button myButton;
    public bool isDeleted;
    

    // ボタン押した時
    public void OnPointerEnter(PointerEventData eventData)
    {
        isDeleted = true;
    }

    // ボタン押し終わった時
    public void OnPointerExit(PointerEventData eventData)
    {
        isDeleted = false;
    }

}
