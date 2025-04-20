using KartGame.KartSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static VehiclePart;
using static VehicleStats;

public class VehicleController : MonoBehaviour
{
    public PlayerVehicleData vehicleData;

    public Transform bodyModelParent;
    public Transform engineMadeParent;
    public Transform[] wheelModelParents;
    public Transform[] mirrorModelParents;

    public GameObject[] bodyDamageEffects;
    public GameObject engineDamageEffect;
    public GameObject wheelDamageEffect;
    public GameObject mirrorDamageEffect;

    public ArcadeKart kart;
    public VehicleStats baseStats;
    private Dictionary<PartType, GameObject> partModels = new Dictionary<PartType, GameObject>();

    void Start()
    {
        kart = GetComponent<ArcadeKart>();

        //���� ������ ���� (���õ� ���� ������ ��������) - > �ȵǴ°Ű��� �̰�... .. ..  
        vehicleData = GameManager.Instance.playerData.GetCurrentVehicle();

        ApplyVehicleParts();

        UpdateDamageVisuals();
    }

    void Update()
    {
        ApplyDamageEffects();
    }

    void ApplyVehicleParts()
    {
        ClearPartModels();

        // baseStats �ʱ�ȭ
        baseStats = ScriptableObject.CreateInstance<VehicleStats>();
        if (baseStats == null)
        {
            Debug.LogWarning("VehicleStats ���� ����. �⺻���� ����մϴ�.");
            baseStats = ScriptableObject.CreateInstance<VehicleStats>();
            // �⺻�� ���� ����
            baseStats.topSpeed = 180f;
            baseStats.acceleration = 8f;
            baseStats.handling = 5f;
            baseStats.braking = 8f;
        }

        // vehicleData�� null���� Ȯ��
        if (vehicleData == null)
        {
            Debug.LogWarning("vehicleData�� null�Դϴ�. �⺻ ���� �����͸� ����մϴ�.");
            vehicleData = new PlayerVehicleData();
        }

        // �θ� Transform Ȯ��
        if (bodyModelParent != null)
            ApplyPartModel(PartType.Body, vehicleData.bodyId, bodyModelParent);
        else
            Debug.LogWarning("bodyModelParent�� �Ҵ���� �ʾҽ��ϴ�.");

        if (engineMadeParent != null)
            ApplyPartModel(PartType.Engine, vehicleData.engineId, engineMadeParent);
        else
            Debug.LogWarning("engineMadeParent�� �Ҵ���� �ʾҽ��ϴ�.");

        // �� ����
        if (wheelModelParents != null && wheelModelParents.Length > 0)
        {
            for (int i = 0; i < wheelModelParents.Length; i++)
            {
                if (wheelModelParents[i] != null)
                    ApplyPartModel(PartType.Wheel, vehicleData.wheelId, wheelModelParents[i], true);
            }
        }
        else
            Debug.LogWarning("wheelModelParents�� �Ҵ���� �ʾҽ��ϴ�.");

        // �̷� ����
        if (mirrorModelParents != null && mirrorModelParents.Length > 0)
        {
            for (int i = 0; i < mirrorModelParents.Length; i++)
            {
                if (mirrorModelParents[i] != null)
                    ApplyPartModel(PartType.Mirror, vehicleData.mirrorId, mirrorModelParents[i], true);
            }
        }
        else
            Debug.LogWarning("mirrorModelParents�� �Ҵ���� �ʾҽ��ϴ�.");

        // īƮ ���� ������Ʈ
        UpdateKartStats();
    }

    void ApplyPartModel(PartType type, int partId, Transform parent, bool isChild = false)
    {
        VehiclePart part = PartsManager.Instance.GetPart(partId);
        if (part != null && part.partModel != null)
        {
            GameObject model = Instantiate(part.partModel, parent);

            if (!isChild)
            {
                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;
                model.transform.localScale = Vector3.one;
            }

            if (partModels.ContainsKey(type))
                partModels[type] = model;
            else
                partModels.Add(type, model);

            baseStats = part.ApplyModifiers(baseStats);
        }
    }

    void ClearPartModels()
    {
        foreach (var model in partModels.Values)
        {
            if (model != null)
                Destroy(model);
        }
        partModels.Clear();
    }

    void UpdateKartStats()
    {
        VehicleStats finalStats = baseStats.GetModifiedStats(
            vehicleData.bodyDamage,
            vehicleData.engineDamage,
            vehicleData.wheelDamage,
            vehicleData.mirrorDamage
            );
        kart.baseStats.TopSpeed = finalStats.topSpeed;
        kart.baseStats.Acceleration = finalStats.acceleration;
        kart.baseStats.Steer = finalStats.handling;
        kart.baseStats.Braking = finalStats.braking;
    }

    void ApplyDamageEffects()
    {
        UpdateKartStats();
        UpdateDamageVisuals();
    }

    //�ջ� �ð� ȿ�� ������Ʈ
    void UpdateDamageVisuals()
    {
        for (int i = 0; i < bodyDamageEffects.Length; i++)
        {
            float damageThreshold = (i + 1) * (100f / bodyDamageEffects.Length);
            bodyDamageEffects[i].SetActive(vehicleData.bodyDamage >= damageThreshold);

        }

        if (engineDamageEffect != null)
            engineDamageEffect.SetActive(vehicleData.engineDamage > 50f);

        if (wheelDamageEffect != null)
            wheelDamageEffect.SetActive(vehicleData.wheelDamage > 50f);

        if (mirrorDamageEffect != null)
            mirrorDamageEffect.SetActive(vehicleData.mirrorDamage > 50f);
    }

    //�浹 �� �ջ� ����
    public void ApplyCollisitionDamage(PartType partType, float damageAmount)
    {
        switch (partType)
        {
            case PartType.Body:
                vehicleData.bodyDamage = Mathf.Min(vehicleData.bodyDamage + damageAmount, 100f);
                break;
            case PartType.Engine:
                vehicleData.engineDamage = Mathf.Min(vehicleData.engineDamage + damageAmount, 100f);
                break;
            case PartType.Wheel:
                vehicleData.wheelDamage = Mathf.Min(vehicleData.wheelDamage + damageAmount, 100f);
                break;
            case PartType.Mirror:
                vehicleData.mirrorDamage = Mathf.Min(vehicleData.mirrorDamage + damageAmount, 100f);
                break;
        }
    }

    // �߰�: ��ǰ�� �ջ� ���� ��ȸ �޼���
    public float GetPartDamage(PartType partType)
    {
        switch (partType)
        {
            case PartType.Body:
                return vehicleData.bodyDamage;
            case PartType.Engine:
                return vehicleData.engineDamage;
            case PartType.Wheel:
                return vehicleData.wheelDamage;
            case PartType.Mirror:
                return vehicleData.mirrorDamage;
            default:
                return 0f;
        }
    }
}