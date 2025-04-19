using KartGame.KartSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static VehiclePart;

public class VehicleController : MonoBehaviour
{
    public PlayerVehicleData vehicleData;

    public Transform bodyModelParent;
    public Transform engineMadeParent;
    public Transform[] wheelModeParents;
    public Transform[] mirrorModelParents;

    public GameObject[] bodyDamageEffects;
    public GameObject engineDamageEffect;
    public GameObject wheelDamageEffect;
    public GameObject mirrorDamageEffect;

    private ArcadeKart kart;
    private VehicleStats baseStats;
    private Dictionary<PartType, GameObject> partModels = new Dictionary<PartType, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        kart = GetComponent<ArcadeKart>();

        //vehicleData = GameManager.Instance.PlayerData.
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
