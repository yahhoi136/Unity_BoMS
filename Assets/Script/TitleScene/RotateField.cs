using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateField : MonoBehaviour
{

    [SerializeField] float angle = 0.03f;

    void Update()
    {
        transform.Rotate(new Vector3(0, angle, 0), Space.Self);
    }
}
