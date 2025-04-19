using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    
    public int money;       //�� �� ��
    public bool isEnd;      //��������
    public int reward;      //����

    void Start()
    {
        isEnd = false;
        if (gameObject == null) Debug.LogError("Null GameManager");
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(isEnd == true)
        {
            money += reward;
        }
    }

    public enum GameState
    {
        Main,
        StageSelect,
        Garage,
        Racing,
        StageComplete,
        GameOver
    }
}
