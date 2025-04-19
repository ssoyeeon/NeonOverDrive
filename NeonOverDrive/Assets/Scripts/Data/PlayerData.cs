using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int currency = 0;

    public int currentVehicleId = 0;
    public List<PlayerVehicleData> vehicles = new List<PlayerVehicleData>();
    public List<int> ownedParts = new List<int>();

    public PlayerData()
    {

    }
}
