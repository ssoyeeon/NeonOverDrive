using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int money;       //�� �� ��
    public bool isEnd;      //��������
    public int reward;      //����

    void Start()
    {
        isEnd = false;
        DontDestroyOnLoad(gameObject);
        if (gameObject == null) Debug.LogError("���� �Ŵ��� �����");
    }

    // Update is called once per frame
    void Update()
    {
        if(isEnd == true)
        {
            money += reward;
        }
    }
}
