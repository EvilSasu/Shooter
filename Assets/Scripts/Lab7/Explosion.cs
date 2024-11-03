using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public ParticleSystem explosion;
    public float explosionRadius = 5f;
    public float explosionForce = 700f; 
    public float damage = 50f;
    private ParticleSystem newParticle;
    void OnCollisionEnter(Collision collision)
    {
        newParticle = Instantiate(explosion, transform.position, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            ObjectData objectData = nearbyObject.GetComponent<ObjectData>();
            if (objectData != null)
            {
                if(nearbyObject.CompareTag("Player")) 
                    objectData.DealDamage(10f, true);
                else
                    objectData.DealDamage(damage, true);
            }

            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        Invoke("DestroyAll", 1.5f);
    }

    private void DestroyAll()
    {
        Destroy(newParticle.gameObject);
        Destroy(gameObject);
    }
}
