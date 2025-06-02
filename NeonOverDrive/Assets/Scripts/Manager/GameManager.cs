using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    public GameState CurrentGameState { get; private set; }
    
    public int money;              //�� �� ��
    public bool isEnd;             //��������
    public int reward = 200;      //����
    public float gameTimer;

    public class SavedVehicleData
    {
        public int vehicleId;
        public int bodyLevel = 1;
        public int engineLevel = 1;
        public int wheelLevel = 1;
        public int mirrorLevel = 1;
    }

    public int playerCurrency = 1000;
    public int selectedVehicleId = 1;
    public List<SavedVehicleData> playerVehicles = new List<SavedVehicleData> ();

    private VehicleController activeVehicleController;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

    }

    private void LoadGameData()
    {
        if(PlayerPrefs.HasKey("PlayerData"))
        {
            string json = PlayerPrefs.GetString("PlayerData");
            JsonUtility.FromJsonOverwrite(json, this);
        }
        else
        {
            SavedVehicleData defaultVehicle = new SavedVehicleData
            {
                vehicleId = 1,
                bodyLevel = 1,
                engineLevel = 1,
                wheelLevel = 1,
                mirrorLevel = 1
            };
            playerVehicles.Add(defaultVehicle);
        }
    }

    public void SaveGameData()
    { 
        // ���� Ȱ��ȭ�� �������� ���׷��̵� ���� ��������
        if (activeVehicleController != null && activeVehicleController.vehicleData != null)
        {
            UpdateVehicleDataFromController();
        }

        // �����͸� JSON���� ��ȯ�Ͽ� ����
        string json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString("PlayerData", json);
        PlayerPrefs.Save();

        Debug.Log("���� ������ ���� �Ϸ�");

    }
    
    private void UpdateVehicleDataFromController()
    {
        VehicleData data = activeVehicleController.vehicleData;

        // ���� ���� ID ã��
        int vehicleId = data.vehicleId;

        // �÷��̾� ���� ��Ͽ��� �ش� ID�� ���� ã��
        SavedVehicleData savedData = GetVehicleData(vehicleId);

        // ���� ���� ������Ʈ
        savedData.bodyLevel = data.bodyLevel;
        savedData.engineLevel = data.engineLevel;
        savedData.wheelLevel = data.wheelLevel;
        savedData.mirrorLevel = data.mirrorLevel;
    }
    
    // ���� ID�� ����� ���� ������ ��������
    public SavedVehicleData GetVehicleData(int vehicleId)
    {
        foreach (var vehicle in playerVehicles)
        {
            if (vehicle.vehicleId == vehicleId)
                return vehicle;
        }

        // ������ ���� ����
        SavedVehicleData newData = new SavedVehicleData { vehicleId = vehicleId };
        playerVehicles.Add(newData);
        return newData;
    }
    
    // ���� �� �ε� �� ȣ�� (���� ���� ��)
    public void InitializeGameScene(VehicleController vehicleController)
    {
        // ��Ʈ�ѷ� ���� ����
        activeVehicleController = vehicleController;

        // ���õ� ������ ���� ������ ��������
        SavedVehicleData vehicleData = GetVehicleData(selectedVehicleId);

        // �����͸� ���� ��Ʈ�ѷ��� ����
        ApplyVehicleData(vehicleData);
    }

    // ���� �����͸� ������ ����
    private void ApplyVehicleData(SavedVehicleData savedData)
    {
        if (activeVehicleController == null || activeVehicleController.vehicleData == null)
            return;

        VehicleData data = activeVehicleController.vehicleData;

        // ���� ����
        data.bodyLevel = savedData.bodyLevel;
        data.engineLevel = savedData.engineLevel;
        data.wheelLevel = savedData.wheelLevel;
        data.mirrorLevel = savedData.mirrorLevel;

        // �ɷ�ġ ������Ʈ
        activeVehicleController.ApplyVehicleStats();

        Debug.Log($"���� #{savedData.vehicleId} ������ ����: �ٵ�={savedData.bodyLevel}, ����={savedData.engineLevel}, ��={savedData.wheelLevel}, �̷�={savedData.mirrorLevel}");
    }

    // ��ǰ ���׷��̵� (UI���� ȣ��)
    public bool UpgradePart(int partIndex, int cost)
    {
        // ��� Ȯ��
        if (playerCurrency < cost)
        {
            Debug.Log("��ȭ�� �����մϴ�!");
            return false;
        }

        // ��� ����
        playerCurrency -= cost;

        // ��ǰ ���׷��̵�
        if (activeVehicleController != null)
        {
            activeVehicleController.UpgradePart(partIndex);
        }

        // ����
        SaveGameData();

        return true;
    }

    public void Update()
    {
        if(SceneManager.GetActiveScene().name == "Stage1Scene" || SceneManager.GetActiveScene().name == "Stage2Scene")
        {
            gameTimer += Time.deltaTime; 
            
        }
        if (SceneManager.GetActiveScene().name != "OutScene" && SceneManager.GetActiveScene().name != "SampleScene")
        {
            gameTimer = 0;
        }

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
