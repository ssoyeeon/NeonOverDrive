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

    private int[] upgradeCosts = { 100, 150, 200, 250 }; // Body, Engine, Wheel, Mirror

    void Start()
    {
        // �� üũ �߰�
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance�� null�Դϴ�!");
            return;
        }

        // ��ư �̺�Ʈ ���� (�� üũ �߰�)
        if (bodyUpgradeButton != null)
        {
            bodyUpgradeButton.onClick.RemoveAllListeners(); // ���� ������ ����
            bodyUpgradeButton.onClick.AddListener(() => UpgradePart(0));
            Debug.Log("Body ��ư �����");
        }
        else Debug.LogWarning("bodyUpgradeButton�� null�Դϴ�!");

        if (engineUpgradeButton != null)
        {
            engineUpgradeButton.onClick.RemoveAllListeners();
            engineUpgradeButton.onClick.AddListener(() => UpgradePart(1));
            Debug.Log("Engine ��ư �����");
        }
        else Debug.LogWarning("engineUpgradeButton�� null�Դϴ�!");

        if (wheelUpgradeButton != null)
        {
            wheelUpgradeButton.onClick.RemoveAllListeners();
            wheelUpgradeButton.onClick.AddListener(() => UpgradePart(2));
            Debug.Log("Wheel ��ư �����");
        }
        else Debug.LogWarning("wheelUpgradeButton�� null�Դϴ�!");

        if (mirrorUpgradeButton != null)
        {
            mirrorUpgradeButton.onClick.RemoveAllListeners();
            mirrorUpgradeButton.onClick.AddListener(() => UpgradePart(3));
            Debug.Log("Mirror ��ư �����");
        }
        else Debug.LogWarning("mirrorUpgradeButton�� null�Դϴ�!");

        UpdateStatsDisplay();
    }

    public void UpgradePart(int partIndex)
    {
        Debug.Log($"UpgradePart ȣ��� - partIndex: {partIndex}");

        // ������ üũ
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance�� null�Դϴ�!");
            return;
        }

        int cost = upgradeCosts[partIndex];
        Debug.Log($"���׷��̵� ���: {cost}, ���� �Ӵ�: {GameManager.Instance.money}");

        if (GameManager.Instance.money >= cost)
        {
            GameManager.Instance.money -= cost;

            if (vehicleController != null)
            {
                vehicleController.UpgradePart(partIndex);
            }
            else
            {
                Debug.LogWarning("vehicleController�� null�Դϴ�!");
            }

            // GameManager �����͵� ������Ʈ
            var vehicleData = GameManager.Instance.GetVehicleData(GameManager.Instance.selectedVehicleId);
            switch (partIndex)
            {
                case 0: vehicleData.bodyLevel++; break;
                case 1: vehicleData.engineLevel++; break;
                case 2: vehicleData.wheelLevel++; break;
                case 3: vehicleData.mirrorLevel++; break;
            }

            GameManager.Instance.SaveGameData();
            Debug.Log($"���׷��̵� �Ϸ� - ��Ʈ: {partIndex}");
        }
        else
        {
            Debug.Log("���� �����մϴ�!");
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