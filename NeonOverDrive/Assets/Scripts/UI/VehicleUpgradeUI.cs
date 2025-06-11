using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VehicleUpgradeUI : MonoBehaviour
{
    public VehicleController vehicleController;

    [Header("UI ���")]
    public Button bodyUpgradeButton;
    public Button engineUpgradeButton;
    public Button wheelUpgradeButton;
    public Button mirrorUpgradeButton;

    public TextMeshProUGUI statsText;

    // Start is called before the first frame update
    void Start()
    {
        // ��ư �̺�Ʈ ����
        if (bodyUpgradeButton != null)
            bodyUpgradeButton.onClick.AddListener(() => UpgradePart(0));

        if (engineUpgradeButton != null)
            engineUpgradeButton.onClick.AddListener(() => UpgradePart(1));

        if (wheelUpgradeButton != null)
            wheelUpgradeButton.onClick.AddListener(() => UpgradePart(2));

        if (mirrorUpgradeButton != null)
            mirrorUpgradeButton.onClick.AddListener(() => UpgradePart(3));

        // �ʱ� ���� ǥ��
        UpdateStatsDisplay();
    }

    // ��ǰ ���׷��̵�
    public void UpgradePart(int partIndex)
    {
        if (vehicleController == null || vehicleController.vehicleData == null)
            return;

        // ��ǰ ���׷��̵�
        vehicleController.UpgradePart(partIndex);

        // UI ������Ʈ
        UpdateStatsDisplay();
    }

    // ���� ǥ�� ������Ʈ
    private void UpdateStatsDisplay()
    {
        if (vehicleController == null || vehicleController.vehicleData == null || statsText == null)
            return;

        VehicleData data = vehicleController.vehicleData;

        statsText.text = $"Car: {data.vehicleName} (ID: {data.vehicleId})\n" +
                         $"Speed: {data.GetFinalSpeed():F1}\n" +
                         $"Acceleration: {data.GetFinalAcceleration():F1}\n" +
                         $"Handling: {data.GetFinalHandling():F1}\n" +
                         $"Breaking: {data.GetFinalBraking():F1}\n\n" +
                         $"Part Levels:\n" +
                         $"Body: {data.bodyLevel}\n" +
                         $"Engine: {data.engineLevel}\n" +
                         $"Wheel: {data.wheelLevel}\n" +
                         $"Mirror: {data.mirrorLevel}";
    }
}
