using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    public GameState CurrentGameState { get; private set; }
    
    public int money;              //총 내 돈
    public bool isEnd;             //끝났는지
    public int reward = 200;      //보상
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
        // 현재 활성화된 차량에서 업그레이드 정보 가져오기
        if (activeVehicleController != null && activeVehicleController.vehicleData != null)
        {
            UpdateVehicleDataFromController();
        }

        // 데이터를 JSON으로 변환하여 저장
        string json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString("PlayerData", json);
        PlayerPrefs.Save();

        Debug.Log("게임 데이터 저장 완료");

    }
    
    private void UpdateVehicleDataFromController()
    {
        VehicleData data = activeVehicleController.vehicleData;

        // 현재 차량 ID 찾기
        int vehicleId = data.vehicleId;

        // 플레이어 차량 목록에서 해당 ID의 차량 찾기
        SavedVehicleData savedData = GetVehicleData(vehicleId);

        // 레벨 정보 업데이트
        savedData.bodyLevel = data.bodyLevel;
        savedData.engineLevel = data.engineLevel;
        savedData.wheelLevel = data.wheelLevel;
        savedData.mirrorLevel = data.mirrorLevel;
    }
    
    // 차량 ID로 저장된 차량 데이터 가져오기
    public SavedVehicleData GetVehicleData(int vehicleId)
    {
        foreach (var vehicle in playerVehicles)
        {
            if (vehicle.vehicleId == vehicleId)
                return vehicle;
        }

        // 없으면 새로 생성
        SavedVehicleData newData = new SavedVehicleData { vehicleId = vehicleId };
        playerVehicles.Add(newData);
        return newData;
    }
    
    // 게임 씬 로드 시 호출 (차량 스폰 후)
    public void InitializeGameScene(VehicleController vehicleController)
    {
        // 컨트롤러 참조 저장
        activeVehicleController = vehicleController;

        // 선택된 차량의 저장 데이터 가져오기
        SavedVehicleData vehicleData = GetVehicleData(selectedVehicleId);

        // 데이터를 차량 컨트롤러에 적용
        ApplyVehicleData(vehicleData);
    }

    // 저장 데이터를 차량에 적용
    private void ApplyVehicleData(SavedVehicleData savedData)
    {
        if (activeVehicleController == null || activeVehicleController.vehicleData == null)
            return;

        VehicleData data = activeVehicleController.vehicleData;

        // 레벨 설정
        data.bodyLevel = savedData.bodyLevel;
        data.engineLevel = savedData.engineLevel;
        data.wheelLevel = savedData.wheelLevel;
        data.mirrorLevel = savedData.mirrorLevel;

        // 능력치 업데이트
        activeVehicleController.ApplyVehicleStats();

        Debug.Log($"차량 #{savedData.vehicleId} 데이터 적용: 바디={savedData.bodyLevel}, 엔진={savedData.engineLevel}, 휠={savedData.wheelLevel}, 미러={savedData.mirrorLevel}");
    }

    // 부품 업그레이드 (UI에서 호출)
    public bool UpgradePart(int partIndex, int cost)
    {
        // 비용 확인
        if (playerCurrency < cost)
        {
            Debug.Log("통화가 부족합니다!");
            return false;
        }

        // 비용 차감
        playerCurrency -= cost;

        // 부품 업그레이드
        if (activeVehicleController != null)
        {
            activeVehicleController.UpgradePart(partIndex);
        }

        // 저장
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
