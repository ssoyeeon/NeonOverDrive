using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    public GameState CurrentGameState { get; private set; }
    
    public int money;              //ÃÑ ³» µ·
    public bool isEnd;             //³¡³µ´ÂÁö
    public int reward = 200;      //º¸»ó
    public float gameTimer;

    public void Update()
    {
        if(SceneManager.GetActiveScene().name == "SampleScene")
        {
            gameTimer += Time.deltaTime; 
            
        }
        if (SceneManager.GetActiveScene().name != "OutScene" && SceneManager.GetActiveScene().name != "SampleScene")
        {
            gameTimer = 0;
        }

    }
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
