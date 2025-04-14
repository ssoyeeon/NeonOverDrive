using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int money;       //총 내 돈
    public bool isEnd;      //끝났는지
    public int reward;      //보상

    void Start()
    {
        isEnd = false;
        DontDestroyOnLoad(gameObject);
        if (gameObject == null) Debug.LogError("게임 매니저 업대오");
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
