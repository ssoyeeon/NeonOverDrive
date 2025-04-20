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
        // 기본 차량 추가
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

    // 현재 차량 데이터 반환
    public PlayerVehicleData GetCurrentVehicle()
    {
        return vehicles.Find(v => v.vehicleId == currentVehicleId);
    }

    // 새 차량 추가 
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

    //부품 구매
    public void AddPart(int partId)
    {
        if (!ownedParts.Contains(partId))
            ownedParts.Add(partId);
    }
}
