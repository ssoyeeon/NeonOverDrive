using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using static VehiclePart;

public class VehicleCollisionHandler : MonoBehaviour
{
    [Header("충돌 설정")]
    public float minCollisitionForce = 5f;
    public float maxCollisitionForce = 50f;
    public float damageMultiplier = 1f;

    [Header("충돌 콜라이더")]
    public Collider frontCollider;
    public Collider rearCollider;
    public Collider leftCollider;
    public Collider rightCollider;
    public Collider roofCollider;

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

        Collider hitCollider = collision.contacts[0].thisCollider;

        float collisionForce = Mathf.Min(collision.relativeVelocity.magnitude, maxCollisitionForce);

        float damageAmount = ((collisionForce - minCollisitionForce) / (maxCollisitionForce - minCollisitionForce)) * 100f * damageMultiplier;

        if (hitCollider == frontCollider)
        {
            vehicleController.ApplyCollisitionDamage(PartType.Engine, damageAmount * 0.7f);
            vehicleController.ApplyCollisitionDamage(PartType.Body, damageAmount * 0.3f);
        }
        else if(hitCollider == rearCollider)
        {
            vehicleController.ApplyCollisitionDamage(PartType.Body, damageAmount);

        }
        else if (hitCollider == leftCollider || hitCollider == rightCollider)
        {
            vehicleController.ApplyCollisitionDamage(PartType.Body, damageAmount * 0.5f);
            vehicleController.ApplyCollisitionDamage(PartType.Wheel, damageAmount * 0.3f);
            vehicleController.ApplyCollisitionDamage(PartType.Mirror, damageAmount * 0.2f);
        }
        else if(hitCollider == roofCollider)
        {
            vehicleController.ApplyCollisitionDamage(PartType.Body, damageAmount * 1.5f);
        }
        else
        {
            vehicleController.ApplyCollisitionDamage(PartType.Body, damageAmount);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
