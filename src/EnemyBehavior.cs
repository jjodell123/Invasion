using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// This class controls enemy  movement and attacking.
public class EnemyBehavior : MonoBehaviour
{
    [SerializeField]
    private StatBar statBar;

    [SerializeField]
    private AudioClip attackSound;

    [SerializeField]
    private int numPatrolPoints = 3;
    [SerializeField]
    private int patrolRadius = 15;

    [SerializeField]
    private float attackDistance = 3f;
    [SerializeField]
    private float chaseDistance = 20f;
    [SerializeField]
    private float maxHealth = 100f;
    [SerializeField]
    private float damageAmountAvg = 20f;
    [SerializeField]
    private float damageVariation = 20f;
    [SerializeField]
    private float timeToDamage = .7f;
    [SerializeField]
    private float attackDuration = 1f;
    [SerializeField]
    private float deathAnimationDuration = 4f;
    [SerializeField]
    private float freezeTime = 7f;
    [SerializeField]
    private float fov = 120;

    [SerializeField]
    private Transform enemyEyes;

    [SerializeField]
    private GameObject deathParticles;
    [SerializeField]
    private GameObject iceBlock;

    private NavMeshAgent agent;

    private Transform player;
    private float currentHealth;
    private PlayerHealth playerHealth;

    private float attackTime = 0f;

    private bool hasDeltDamage = false;
    private bool isFrozen = false;
    private bool isDying = false;


    private enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }

    private EnemyState currentState;

    private Vector3[] patrolPoints;
    private int currentPatrolPoint = 0;
    private bool disablePatrol = false;

    private LevelManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        // Init enemy health bar
        if (statBar == null)
        {
            statBar = transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<StatBar>();
        }
        currentHealth = maxHealth;
        statBar.SetMaxValue(maxHealth);

        // Init player info
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        playerHealth = player.GetComponent<PlayerHealth>();

        // Init nav mesh
        agent = GetComponent<NavMeshAgent>();

        levelManager = FindObjectOfType<LevelManager>();

        // Starts enemy in patrol state
        currentState = EnemyState.Patrol;

        patrolPoints = new Vector3[numPatrolPoints];

        // Checks if the patrol points may fall off of the map
        float xRangeMin = transform.position.x - patrolRadius;
        if (xRangeMin < levelManager.GetXMin())
            xRangeMin = levelManager.GetXMin();

        float xRangeMax = transform.position.x + patrolRadius;
        if (xRangeMax > levelManager.GetXMax())
            xRangeMax = levelManager.GetXMax();

        float zRangeMin = transform.position.z - patrolRadius;
        if (zRangeMin < levelManager.GetZMin())
            zRangeMin = levelManager.GetZMin();

        float zRangeMax = transform.position.z + patrolRadius;
        if (zRangeMax > levelManager.GetZMax())
            zRangeMax = levelManager.GetZMax();

        // Creates patrol points
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            patrolPoints[i] = new Vector3(Random.Range(xRangeMin, xRangeMax), 0, Random.Range(zRangeMin, zRangeMax));
        }
    }

    /*
     * Update is called once per frame
     */
    void Update()
    {
        // Checks for if the enemy can move or attack.
        if (!playerHealth.GetPlayerDead() && !isDying && !isFrozen)
        {
            // Destroy if the fall below the field
            if (transform.position.y < -2f)
                DestroyEnemy();

            // Checks if the enemy is dead
            if (currentHealth < maxHealth)
                disablePatrol = true;

            // Handle enemy game state
            switch (currentState)
            {
                case EnemyState.Patrol:
                    DoPatrolState();
                    break;
                case EnemyState.Chase:
                    DoChaseState();
                    break;
                case EnemyState.Attack:
                    DoAttackState();
                    break;
            }
        }
    }

    /* 
     * Handles the patrol mode. Switches to other modes as necessary
     */
    private void DoPatrolState()
    {
        patrolPoints[currentPatrolPoint].y = transform.position.y;

        agent.SetDestination(patrolPoints[currentPatrolPoint]);
        agent.stoppingDistance = 0;

        // Sets the next patrol point when the enemy has moved to the current one
        if (Vector3.Distance(transform.position, patrolPoints[currentPatrolPoint]) < 1)
        {
            FindDestination();
        }

        // Maintains walk animation until the enemy sees the player
        if (!disablePatrol && !CheckPlayerInFOV()) // Track player
        {
            gameObject.GetComponent<Animator>().SetInteger("activeState", 0);

        }
        else // Start attack process
        {
            currentState = EnemyState.Chase;
        }
    }

    /* 
     * Handles the chase state.
     */
    private void DoChaseState()
    {
        // Gets the grounded position to prevent the player from jumping over enemies
        Vector3 playerGroundedPosition = player.gameObject.GetComponent<PlayerController>().GetGroundedPosition();

        agent.stoppingDistance = attackDistance;
        agent.SetDestination(playerGroundedPosition);

        // If the player escapes the enemy, go back to patrol state
        if (Vector3.Distance(transform.position, playerGroundedPosition) > chaseDistance && !disablePatrol)
        {
            currentState = EnemyState.Patrol;
        }
        else if (Vector3.Distance(transform.position, playerGroundedPosition) > attackDistance) // Track player
        {
            gameObject.GetComponent<Animator>().SetInteger("activeState", 0);
        }
        else // Start attack process when within range
        {
            gameObject.GetComponent<Animator>().SetInteger("activeState", 1);
            currentState = EnemyState.Attack;
        }
    }

    /* 
     * Handles the attack state.
     */
    private void DoAttackState()
    {
        attackTime += Time.deltaTime;

        // When to deal damage based on the animation
        if (attackTime > timeToDamage && !hasDeltDamage) 
        {
            AudioSource.PlayClipAtPoint(attackSound, transform.position);
            hasDeltDamage = true;
            playerHealth.TakeDamage(damageAmountAvg + Random.Range(-damageVariation, damageVariation));
        }

        // Resets attack abilities when done
        if (attackTime > attackDuration) 
        {
            hasDeltDamage = false;
            currentState = EnemyState.Chase;
            attackTime = 0;
            gameObject.GetComponent<Animator>().SetInteger("activeState", 0);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (!isFrozen)
        {
            currentHealth -= damageAmount;
            statBar.UpdateValue(-damageAmount);
            if (currentHealth <= 0 && !isDying)
                DestroyEnemy();
        }
    }

    private void DestroyEnemy()
    {
        // Allows gameobject to fall further into ground
        agent.SetDestination(transform.position);
        // Starts die animation
        GetComponent<Animator>().SetInteger("activeState", 2);
        GetComponent<Animator>().applyRootMotion = true;

        isDying = true;

        FindObjectOfType<LevelManager>().IncreaseScore();
        
        Destroy(gameObject, deathAnimationDuration);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isFrozen)
        {
            if (other.CompareTag("Projectile") && gameObject.tag == "Enemy")
            {
                TakeDamage(other.GetComponent<ArrowDamage>().GetDamage());
                print(other.GetComponent<ArrowDamage>().GetDamage());
                disablePatrol = true;
            }
            else if (other.CompareTag("Projectile"))
            {
                disablePatrol = true;
            }
            else if (other.CompareTag("FreezeArrow"))
            {
                // Make ice block
                GameObject ice = Instantiate(iceBlock, transform.position + (transform.forward * .6f) +
                    (transform.right * -.5f), transform.localRotation * Quaternion.Euler(90, 40, 0));

                //transform.position = ice.transform.position;
                agent.SetDestination(transform.position);
                agent.isStopped = true;

                // Make the enemy stand still
                GetComponent<Animator>().SetInteger("activeState", 3);
                isFrozen = true;

                // Stop damage
                hasDeltDamage = false;
                attackTime = 0;

                // Destroy arrow
                Destroy(other);

                // Set removal time
                float removalTime = other.GetComponent<ArrowDamage>().GetPercentDrawBack();
                print(removalTime);
                Destroy(ice, freezeTime * removalTime);
                Invoke("Unfreeze", freezeTime * removalTime);

                disablePatrol = true;
            } else if (other.CompareTag("Bomb"))
            {
                disablePatrol = true;
            }
        }
    }

    private void FindDestination()
    {
        currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
        
    }

    private void OnDestroy()
    {
        if (deathParticles != null && !LevelManager.isGameOver) // Creates particles on death.
        {
            Instantiate(deathParticles, transform.position, transform.rotation);
        }
    }

    private void Unfreeze()
    {
        isFrozen = false;
        agent.isStopped = false;
    }

    private bool CheckPlayerInFOV()
    {
        RaycastHit hit;

        // Direction to player waist position. It defaults to the feet which is not consistently
        // hit with this model.
        Vector3 directionToPlayer = player.transform.position - enemyEyes.position + Vector3.up;

        if (Vector3.Angle(directionToPlayer, enemyEyes.forward) <= (fov / 2))
        {
            if (Physics.Raycast(enemyEyes.position, directionToPlayer, out hit, chaseDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
