using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Vehicle Stats", menuName = "Racing Game/Vehicle Stat")]
public class VehicleStats : MonoBehaviour
{
    [Header("기본성능")]
    [Tooltip("최고 속도 (km/h")]
    public float topSpeed = 180f;

    [Tooltip("가속력 (0~100km/h 도달 시간의 역수")]
    public float acceleration = 8f;

    [Tooltip("핸들링(코너링 능력)")]
    public float handling = 5f;

    [Tooltip("제동력(100~0km/h 감속 시간의 역수")]
    public float braking = 8f;

    //근데 이거 왜 복제본으로 하는거에요....?????????? 그냥 하면 되는거 아닌가오..? ㅠ
    public VehicleStats GetModifiedStats(float bodyDamage, float engineDamage, float wheelDamage, float mirrorDamage)
    {
        VehicleStats modifiedStats = Instantiate(this);

        float engineFactor = 1f - (engineDamage / 100f);
        modifiedStats.topSpeed += engineFactor;
        modifiedStats.acceleration *= engineFactor;

        float wheelFactor = 1f - (wheelDamage / 100f);
        modifiedStats.handling *= wheelFactor;
        modifiedStats.braking *= wheelFactor;

        float bodyFactor = 1f - (bodyDamage / 150f);
        modifiedStats.topSpeed *= bodyFactor;
        modifiedStats.acceleration *= bodyFactor;
        modifiedStats.handling *= bodyFactor;

        float mirrorFactor = 1f - (mirrorDamage / 400f);
        modifiedStats.handling *= mirrorFactor;

        return modifiedStats;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
