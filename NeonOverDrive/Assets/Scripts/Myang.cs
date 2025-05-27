using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Myang : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(Vector3.forward, 0.05f * Time.deltaTime);
    }
}
