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

    // Start is called before the first frame update
    void Start()
    {
        // 버튼 이벤트 연결
        if (bodyUpgradeButton != null)
            bodyUpgradeButton.onClick.AddListener(() => UpgradePart(0));

        if (engineUpgradeButton != null)
            engineUpgradeButton.onClick.AddListener(() => UpgradePart(1));

        if (wheelUpgradeButton != null)
            wheelUpgradeButton.onClick.AddListener(() => UpgradePart(2));

        if (mirrorUpgradeButton != null)
            mirrorUpgradeButton.onClick.AddListener(() => UpgradePart(3));

        // 초기 스탯 표시
        UpdateStatsDisplay();
    }

    // 부품 업그레이드
    public void UpgradePart(int partIndex)
    {
        if (vehicleController == null || vehicleController.vehicleData == null)
            return;

        // 부품 업그레이드
        vehicleController.UpgradePart(partIndex);

        // UI 업데이트
        UpdateStatsDisplay();
    }

    // 스탯 표시 업데이트
    private void UpdateStatsDisplay()
    {
        if (vehicleController == null || vehicleController.vehicleData == null || statsText == null)
            return;

        VehicleData data = vehicleController.vehicleData;

        statsText.text = $"차량: {data.vehicleName} (ID: {data.vehicleId})\n" +
                         $"속도: {data.GetFinalSpeed():F1}\n" +
                         $"가속: {data.GetFinalAcceleration():F1}\n" +
                         $"핸들링: {data.GetFinalHandling():F1}\n" +
                         $"제동: {data.GetFinalBraking():F1}\n\n" +
                         $"부품 레벨:\n" +
                         $"바디: {data.bodyLevel}\n" +
                         $"엔진: {data.engineLevel}\n" +
                         $"휠: {data.wheelLevel}\n" +
                         $"미러: {data.mirrorLevel}";
    }
}
