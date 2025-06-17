using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VehicleUpgradeUI : MonoBehaviour
{
    public VehicleController vehicleController;

    [Header("UI 요소")]
    public Button bodyUpgradeButton;
    public Button engineUpgradeButton;
    public Button wheelUpgradeButton;
    public Button mirrorUpgradeButton;
    public TextMeshProUGUI statsText;

    private int[] upgradeCosts = { 100, 150, 200, 250 }; // Body, Engine, Wheel, Mirror

    void Start()
    {
        // 널 체크 추가
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance가 null입니다!");
            return;
        }

        // 버튼 이벤트 연결 (널 체크 추가)
        if (bodyUpgradeButton != null)
        {
            bodyUpgradeButton.onClick.RemoveAllListeners(); // 기존 리스너 제거
            bodyUpgradeButton.onClick.AddListener(() => UpgradePart(0));
            Debug.Log("Body 버튼 연결됨");
        }
        else Debug.LogWarning("bodyUpgradeButton이 null입니다!");

        if (engineUpgradeButton != null)
        {
            engineUpgradeButton.onClick.RemoveAllListeners();
            engineUpgradeButton.onClick.AddListener(() => UpgradePart(1));
            Debug.Log("Engine 버튼 연결됨");
        }
        else Debug.LogWarning("engineUpgradeButton이 null입니다!");

        if (wheelUpgradeButton != null)
        {
            wheelUpgradeButton.onClick.RemoveAllListeners();
            wheelUpgradeButton.onClick.AddListener(() => UpgradePart(2));
            Debug.Log("Wheel 버튼 연결됨");
        }
        else Debug.LogWarning("wheelUpgradeButton이 null입니다!");

        if (mirrorUpgradeButton != null)
        {
            mirrorUpgradeButton.onClick.RemoveAllListeners();
            mirrorUpgradeButton.onClick.AddListener(() => UpgradePart(3));
            Debug.Log("Mirror 버튼 연결됨");
        }
        else Debug.LogWarning("mirrorUpgradeButton이 null입니다!");

        UpdateStatsDisplay();
    }

    public void UpgradePart(int partIndex)
    {
        Debug.Log($"UpgradePart 호출됨 - partIndex: {partIndex}");

        // 안전성 체크
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance가 null입니다!");
            return;
        }

        int cost = upgradeCosts[partIndex];
        Debug.Log($"업그레이드 비용: {cost}, 현재 머니: {GameManager.Instance.money}");

        if (GameManager.Instance.money >= cost)
        {
            GameManager.Instance.money -= cost;

            if (vehicleController != null)
            {
                vehicleController.UpgradePart(partIndex);
            }
            else
            {
                Debug.LogWarning("vehicleController가 null입니다!");
            }

            // GameManager 데이터도 업데이트
            var vehicleData = GameManager.Instance.GetVehicleData(GameManager.Instance.selectedVehicleId);
            switch (partIndex)
            {
                case 0: vehicleData.bodyLevel++; break;
                case 1: vehicleData.engineLevel++; break;
                case 2: vehicleData.wheelLevel++; break;
                case 3: vehicleData.mirrorLevel++; break;
            }

            GameManager.Instance.SaveGameData();
            Debug.Log($"업그레이드 완료 - 파트: {partIndex}");
        }
        else
        {
            Debug.Log("돈이 부족합니다!");
        }

        UpdateStatsDisplay();
    }

    private void UpdateStatsDisplay()
    {
        if (vehicleController == null || vehicleController.vehicleData == null || statsText == null)
            return;

        VehicleData data = vehicleController.vehicleData;

        statsText.text = $"Car: {data.vehicleName}\n" +
                         $"Speed: {data.GetFinalSpeed():F1}\n" +
                         $"Acceleration: {data.GetFinalAcceleration():F1}\n" +
                         $"Handling: {data.GetFinalHandling():F1}\n" +
                         $"Braking: {data.GetFinalBraking():F1}\n\n" +
                         $"Levels: B{data.bodyLevel} E{data.engineLevel} W{data.wheelLevel} M{data.mirrorLevel}\n" +
                         $"Money: ${GameManager.Instance.money}";
    }

    void Update()
    {
        UpdateStatsDisplay();
    }
}