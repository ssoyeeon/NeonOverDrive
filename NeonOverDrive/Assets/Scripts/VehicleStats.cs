using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Vehicle Stats", menuName = "Racing Game/Vehicle Stat")]
public class VehicleStats : MonoBehaviour
{
    [Header("�⺻����")]
    [Tooltip("�ְ� �ӵ� (km/h")]
    public float topSpeed = 180f;

    [Tooltip("���ӷ� (0~100km/h ���� �ð��� ����")]
    public float acceleration = 8f;

    [Tooltip("�ڵ鸵(�ڳʸ� �ɷ�)")]
    public float handling = 5f;

    [Tooltip("������(100~0km/h ���� �ð��� ����")]
    public float braking = 8f;

    //�ٵ� �̰� �� ���������� �ϴ°ſ���....?????????? �׳� �ϸ� �Ǵ°� �ƴѰ���..? ��
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
