using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreparationController : MonoBehaviour
{

    [SerializeField, NotEditable] GameObject go;
    bool _isRetried;
    public bool IsRetried { get { return _isRetried; } set { _isRetried = value; } }

    void Start()
    {
        // Retry用にDataForRetryオブジェクトにデータを保存。
        if (!GameObject.Find("DataForRetry"))
        {
            go = new GameObject();
            go.name = "DataForRetry";
            go.AddComponent<DataForRetry>();
            DontDestroyOnLoad(go);

            IsRetried = false;
        }
        else
        {
            go = GameObject.Find("DataForRetry");
            IsRetried = true;
        }
    }

    public void battleStart()
    {
        SceneManager.LoadScene("BattleScene");
    }

}
