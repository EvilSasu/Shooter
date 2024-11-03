using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ZombieController : MonoBehaviour
{
    public Transform player;
    public float chaseSpeed = 3f;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public ZombieSpawner spawner;

    private bool isChasing = false;
    private bool isAttacking = false;
    private float lastAttackTime = -Mathf.Infinity;
    private Animator animator;
    private bool returningToSpawner = false;
    private Vector3 spawnerPosition;
    [SerializeField] private Material aggroMaterial;
    [SerializeField] private List<SkinnedMeshRenderer> renderers;

    private ObjectData health;  
    private void Start()
    {
        health = GetComponent<ObjectData>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (health.isDead)
        {
            if(spawner != null)
            {
                spawner.zombies.Remove(this.gameObject);
                Destroy(this.gameObject);
            }
        }
        if (returningToSpawner)
        {
            ReturnToSpawnerMovement();
        }
        else
        {
            if (health.currentHealth != health.maxHealth) isChasing = true;
            if (isChasing)
            {
                ChasePlayer();
                CheckAttackRange();
            }
        }
    }

    public bool GetZombieReturningToSpawner()
    {
        return returningToSpawner;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isChasing = true;
            AddAgroMaterial();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isAttacking = false;
        }
    }

    private void AddAgroMaterial()
    {
        if (renderers.Count <= 0) return;
        foreach(SkinnedMeshRenderer renderer in renderers)
        {
            if (renderer.materials.Length > 1) continue;

            Material[] currentMaterials = renderer.materials;
            if (System.Array.Exists(currentMaterials, material => material != aggroMaterial))
            {
                Material[] newMaterials = new Material[currentMaterials.Length + 1];
                currentMaterials.CopyTo(newMaterials, 0);
                newMaterials[currentMaterials.Length] = aggroMaterial;
                renderer.materials = newMaterials;
            }
        }
    }

    private void ChasePlayer()
    {
        if (!isAttacking)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if(distanceToPlayer > attackRange)
            {
                animator.SetTrigger("Run");
                Vector3 direction = (player.position - transform.position).normalized;
                transform.position += direction * chaseSpeed * Time.deltaTime;
                Vector3 lookDirection = new Vector3(direction.x, 0, direction.z);
                if (lookDirection != Vector3.zero)
                {
                    Quaternion rotation = Quaternion.LookRotation(lookDirection);
                    transform.rotation = rotation;
                }
            }           
        }
    }

    private void CheckAttackRange()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        player.GetComponent<ObjectData>().DealDamage(10f, false);
        animator.SetTrigger("Attack");
        Invoke("ResetAttack", 1f);
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }

    public void ReturnToSpawner(Vector3 position)
    {
        spawnerPosition = position;
        returningToSpawner = true;
    }

    private void ReturnToSpawnerMovement()
    {
        Vector3 direction = (spawnerPosition - transform.position).normalized;
        transform.position += direction * chaseSpeed * Time.deltaTime;

        Vector3 lookDirection = new Vector3(direction.x, 0, direction.z);
        if (lookDirection != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = rotation;
        }

        if (Vector3.Distance(transform.position, spawnerPosition) < 0.5f)
        {
            returningToSpawner = false;
        }
    }
}
