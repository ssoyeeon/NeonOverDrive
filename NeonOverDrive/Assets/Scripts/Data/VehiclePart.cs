using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Vehicle Part", menuName = "Racing Game/VehiclePart")]
public class VehiclePart : ScriptableObject
{
    [Header("�⺻ ����")]
    public int partId;
    public string partName;
    public PartType partType;
    public Sprite icon;
    public int price;

    [Header("���� ������")]
    [Range(-50f, 50f)]
    public float speedModifier;
    [Range(-50f, 50f)]
    public float accelerationModidfier;
    [Range(-50f, 50f)]
    public float handlingModifier;
    [Range(-50f, 50f)]
    public float brakingModifier;

    [Header("�ð��� ���")]
    public GameObject partModel;

    [Header("Ư�� �Ӽ�")]
    //��ǰ Ÿ�Ժ� Ư�� �Ӽ�
    //[ShowIf("partType", PartType.Engine)]
    public float enginePower;

    public float grip;

    public float areodynamicEffect;

    public float weight;

    public VehicleStats ApplyModifiers(VehicleStats baseStats)
    {
        VehicleStats modifiedStats = Instantiate(baseStats);

        // ������� ����
        modifiedStats.topSpeed *= (1 + speedModifier / 100f);
        modifiedStats.acceleration *= (1 + accelerationModidfier / 100f);
        modifiedStats.handling *= (1 + handlingModifier / 100f);
        modifiedStats.braking *= (1 + brakingModifier / 100f);

        return modifiedStats;
    }


    public enum PartType
    {
        Body,
        Engine,
        Wheel,
        Mirror
    }
}
