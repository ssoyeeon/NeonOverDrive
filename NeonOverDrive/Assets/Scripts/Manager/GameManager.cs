using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    public GameState CurrentGameState { get; private set; }

    
    public int money;       //ÃÑ ³» µ·
    public bool isEnd;      //³¡³µ´ÂÁö
    public int reward;      //º¸»ó

    private void Awake()
    {
        if(_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

    }
    public void ChangeGameState(GameState newState)
    {
        CurrentGameState = newState;
        
    }
    public void GoToGarage()
    {
        ChangeGameState(GameState.Garage);
    }
    public void GoToStageSelect()
    {
        ChangeGameState(GameState.StageSelect);
    }
    void Start()
    {
        isEnd = false;
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
