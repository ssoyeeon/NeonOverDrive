using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMove : MonoBehaviour
{
    public float carMove;       //�Ϲ� �� ��
    public float moveSpeed;     //�� ���ӵ�
    public float downCarMove;   //���� �ڷ� �� �� ��
    public GameObject car;
    public Rigidbody carRigi;

    void Start()
    {
        
    }

    void Update()
    {
        carRigi.AddForce(Vector3.up * carMove, ForceMode.Force);

        if(Input.GetKeyDown(KeyCode.W))
        {
            carRigi.AddForce(Vector3.up * moveSpeed * carMove, ForceMode.Force);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            carRigi.AddForce(Vector3.down * moveSpeed * carMove, ForceMode.Force);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            carRigi.AddForce(Vector3.left * moveSpeed * carMove, ForceMode.Force);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            carRigi.AddForce(Vector3.right * moveSpeed * carMove, ForceMode.Force);
        }
    }
}
