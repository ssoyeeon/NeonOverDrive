using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData : MonoBehaviour
{
    public int currency = 0;

    public int currentVehicleId = 0;
    public List<PlayerVehicleData> vehicles = new List<PlayerVehicleData>();
    public List<int> ownedParts = new List<int>();

    public PlayerData()
    {
        // �⺻ ���� �߰�
        vehicles.Add(new PlayerVehicleData
        {
            vehicleId = 0,
            bodyId = 0,
            engineId = 0,
            wheelId = 0,
            mirrorId = 0,
            bodyDamage = 0,
            engineDamage = 0,
            wheelDamage = 0,
            mirrorDamage = 0
        });
    }

    // ���� ���� ������ ��ȯ
    public PlayerVehicleData GetCurrentVehicle()
    {
        return vehicles.Find(v => v.vehicleId == currentVehicleId);
    }

    // �� ���� �߰� 
    public void AddVehicle(int vehicleId, int bodyId, int engineId, int wheelId,int mirrorId)
    {
        vehicles.Add(new PlayerVehicleData
        {
            vehicleId = vehicleId,
            bodyId = bodyId,
            engineId = engineId,
            wheelId = wheelId,
            mirrorId = mirrorId,
            bodyDamage = 0,
            engineDamage = 0,
            wheelDamage = 0,
            mirrorDamage = 0
        });
    }

    //��ǰ ����
    public void AddPart(int partId)
    {
        if (!ownedParts.Contains(partId))
            ownedParts.Add(partId);
    }
}
