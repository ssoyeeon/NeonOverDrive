using static VehiclePart;
using UnityEngine;

public class VehicleCollisionHandler : MonoBehaviour
{
    [Header("�浹 ����")]
    public float minCollisitionForce = 5f;
    public float maxCollisitionForce = 50f;
    public float damageMultiplier = 1f;

    [Header("�浹 ���� ����")]
    public float frontAngle = 45f;    // ���� ���� ����
    public float rearAngle = 45f;     // �ĸ� ���� ����
    private float roofHeightThreshold = 1.5f; // ���� �浹 ���� ���� �Ӱ谪

    [Header("�����")]
    public bool showDebugLogs = true; // ����� �α� ǥ�� ����

    private VehicleController vehicleController;
    private Rigidbody rb;

    void Start()
    {
        vehicleController = GetComponent<VehicleController>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude < minCollisitionForce)
            return;

        // �浹 ���� (ù ��° ������ ���)
        ContactPoint contact = collision.contacts[0];

        // ���� ���� �������� �浹 ���� ���
        Vector3 localImpact = transform.InverseTransformPoint(contact.point);

        // �浹 ���� ���� ��� (XZ ��鿡��)
        float angle = Mathf.Atan2(localImpact.x, localImpact.z) * Mathf.Rad2Deg;
        float absAngle = Mathf.Abs(angle); // ���밪 (0-180��)

        // ���� �浹 üũ (Y ���� Ư�� �Ӱ谪 �̻�)
        bool isRoofImpact = localImpact.y > roofHeightThreshold;

        float collisionForce = Mathf.Min(collision.relativeVelocity.magnitude, maxCollisitionForce);
        float damageAmount = ((collisionForce - minCollisitionForce) / (maxCollisitionForce - minCollisitionForce)) * 100f * damageMultiplier;

        string impactArea = "��Ÿ";

        // ���⿡ ���� �浹 ���� ����
        if (isRoofImpact)
        {
            vehicleController.ApplyCollisitionDamage(PartType.Body, damageAmount * 1.5f);
            impactArea = "����";
        }
        else if (absAngle < frontAngle) // ���� �浹
        {
            vehicleController.ApplyCollisitionDamage(PartType.Engine, damageAmount * 0.7f);
            vehicleController.ApplyCollisitionDamage(PartType.Body, damageAmount * 0.3f);
            impactArea = "����";
        }
        else if (absAngle > 180 - rearAngle) // �Ĺ� �浹
        {
            vehicleController.ApplyCollisitionDamage(PartType.Body, damageAmount);
            impactArea = "�Ĺ�";
        }
        else // ���� �浹
        {
            vehicleController.ApplyCollisitionDamage(PartType.Body, damageAmount * 0.5f);
            vehicleController.ApplyCollisitionDamage(PartType.Wheel, damageAmount * 0.3f);
            vehicleController.ApplyCollisitionDamage(PartType.Mirror, damageAmount * 0.2f);

            // ����/������ ����
            if (localImpact.x < 0)
                impactArea = "����";
            else
                impactArea = "������";
        }

        // ����� ���� �α�
        if (showDebugLogs)
        {
            Debug.Log($"[�浹 ����] ����: {impactArea}, ����: {angle:F1}��, ��ġ: {localImpact}, ��: {collisionForce:F1}, ������: {damageAmount:F1}%");
        }
    }
}