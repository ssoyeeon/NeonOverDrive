using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Myang : MonoBehaviour
{
    public float speed = 5f;
    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(Vector3.forward, speed * Time.deltaTime);
    }
}
