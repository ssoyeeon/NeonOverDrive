using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static VehiclePart;

public class PartsManager : MonoBehaviour
{
    private static PartsManager _instance;
    public static PartsManager Instance => _instance;

    private Dictionary<int, VehiclePart> allParts = new Dictionary<int, VehiclePart>();

    private void Awake()
    {
        if(_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        //모든 부품 로드
        //LoadAllParts();
    }

    public VehiclePart GetPart(int partId)
    {
        if (allParts.ContainsKey(partId))
            return allParts[partId];
        return null;
    }

    public List<VehiclePart> GetPartsByType(PartType type)
    {
        return allParts.Values.Where(p => p.partType == type).ToList();
    }

    private void LoadAllParts()
    {
        VehiclePart[] parts = Resources.LoadAll<VehiclePart>("Parts");
        foreach(var part in parts)
        {
            allParts[part.partId] = part;
        }
    }
}
