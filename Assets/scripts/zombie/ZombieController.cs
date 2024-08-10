using System.Collections;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    [Header("Elements")]
    Rigidbody2D rb; // Reference to the Rigidbody2D component
    GameObject player; // Reference to the player GameObject
    CircleCollider2D circleCollider; // Reference to the CircleCollider2D component
    [SerializeField] LayerMask playerMask; // Layer mask to detect the player
    [SerializeField] LayerMask blockVisionMask; // Layer mask to block vision
    [SerializeField] BloodSprayer bloodSprayer; // Reference to the BloodSprayer component
    [SerializeField] DropShadow dropShadow; // Reference to the DropShadow component

    [Header("Data")]
    [SerializeField] float turnSpeed = 360f; // Speed at which the zombie turns
    [SerializeField] float moveSpeed = 1f; // Speed at which the zombie moves
    [SerializeField] float attackSpeed = 1f; // Time between attacks
    [SerializeField] float maxSightRange = 10f; // Maximum range at which the zombie can see the player
    [SerializeField] float minSightRange = 3f; // Minimum range at which the zombie can see the player
    [SerializeField] float sightArc = 60f; // Sight arc within which the zombie can see the player
    [SerializeField] float attackDistance = 0.25f; // Distance at which the zombie can attack the player
    [SerializeField] float attackRadius = 0.4f; // Radius for detecting the player to attack
    [SerializeField] float attackDetectRadius = 0.2f; // Radius for detecting the player to start attacking
    [SerializeField] float dropShadowOffsetOnDeath = 0.02f; // Offset for the drop shadow on death
    [SerializeField] int damage = 1; // Damage dealt by the zombie
    [SerializeField] int health = 3; // Health of the zombie
    bool isAttacking = false; // Flag to check if the zombie is attacking
    bool isShot = false; // Flag to check if the zombie is shot

    [Header("Audio")]
    [SerializeField] PlayRandomSound hurtSounds;
    [SerializeField] PlayRandomSound deathSounds;
    [SerializeField] PlayRandomSound attackSounds; //start attack
    [SerializeField] PlayRandomSound dealDamageSounds; //connect attack
    [SerializeField] PlayRandomSound alertSounds;
    [SerializeField] PlayRandomSound stepSounds;



    Vector2 lastKnownPlayerPosition; // Last known position of the player
    bool hasLastKnownPosition = false; // Flag to check if the zombie has a last known position

    enum States { idle, attack, dead }; // States for the zombie
    States curState = States.idle; // Current state of the zombie

    private void Awake()
    {
        // Get components and references
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        // Update the zombie's behavior based on the current state
        switch (curState)
        {
            case States.idle:
                UpdateIdleState();
                break;
            case States.attack:
                UpdateAttackState();
                break;
        }
    }

    void SetIdleState()
    {
        // Set the zombie to idle state and stop its movement
        curState = States.idle;
        rb.velocity = Vector2.zero;
    }

    void SetAttackState()
    {
        // Set the zombie to attack state
        curState = States.attack;
        alertSounds.PlaySound();
    }

    void SetDeadState()
    {
        // Set the zombie to dead state, disable its collider, stop its movement, and destroy it
        curState = States.dead;
        rb.velocity = Vector2.zero;
        circleCollider.enabled = false;
        dropShadow.CalculateOffsetDirection(dropShadowOffsetOnDeath);
        Destroy(gameObject);
    }

    void UpdateIdleState()
    {
        // In idle state, check if the zombie can see the player
        if (CanSeePlayer())
        {
            SetAttackState();
            lastKnownPlayerPosition = player.transform.position;
            hasLastKnownPosition = true;
        }
    }

    void UpdateAttackState()
    {
        // In attack state, update the last known player position if the zombie can see the player
        if (CanSeePlayer())
        {
            lastKnownPlayerPosition = player.transform.position;
            hasLastKnownPosition = true;
        }
        else if (hasLastKnownPosition)
        {
            // If the zombie has a last known position, move towards it
            MoveTowardsLastKnownPosition();
            return;
        }

        // If the zombie can't see the player and hasn't been shot, return to idle state
        if (!CanSeePlayer() && !isShot)
        {
            SetIdleState();
            return;
        }

        // Handle attacking and moving towards the player
        if (isAttacking)
        {
            rb.velocity = Vector2.zero;
        }
        else if (CanAttackPlayer(attackDetectRadius))
        {
            StartAttack();
        }
        else
        {
            Vector2 directionToPlayer = player.transform.position - transform.position;
            directionToPlayer.Normalize();
            FaceDirection(directionToPlayer);
            rb.velocity = directionToPlayer * moveSpeed;
        }
    }

    void MoveTowardsLastKnownPosition()
    {
        // Move the zombie towards the last known player position
        Vector2 directionToLastKnownPosition = lastKnownPlayerPosition - (Vector2)transform.position;
        float distanceToLastKnownPosition = directionToLastKnownPosition.magnitude;

        if (distanceToLastKnownPosition < 0.1f) // When the zombie is close enough to the last known position
        {
            hasLastKnownPosition = false;
            SetIdleState();
            return;
        }

        directionToLastKnownPosition.Normalize();
        FaceDirection(directionToLastKnownPosition);
        rb.velocity = directionToLastKnownPosition * moveSpeed;
    }

    void StartAttack()
    {
        if (player.GetComponent<PlayerController>().isDead) { return; }
        // Start the attack if not already attacking
        if (isAttacking)
        {
            return;
        }
        isAttacking = true;
        attackSounds.PlaySound();
        //animation todo here
        Invoke("FinishAttack", attackSpeed);
    }

    void FinishAttack()
    {
        // Finish the attack and check if the player can be attacked
        if (CanAttackPlayer(attackRadius))
        {
            player.GetComponent<PlayerController>().Hurt(damage);
            dealDamageSounds.PlaySound();

        }
        isAttacking = false;
    }

    bool CanAttackPlayer(float radius)
    {
        // Check if the player is within the attack radius
        Collider2D[] circleDetection = Physics2D.OverlapCircleAll(transform.position + transform.up * attackRadius, radius, playerMask);
        if (circleDetection.Length != 0)
        {
            for (int i = 0; i < circleDetection.Length; i++)
            {
                if (circleDetection[i].CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    bool FaceDirection(Vector2 direction)
    {
        // Face the given direction smoothly
        float angleDifference = Vector2.SignedAngle(transform.up, direction);
        bool facingReached = false;
        float turnAmount = turnSpeed * Time.deltaTime;
        if (turnAmount > Mathf.Abs(angleDifference))
        {
            turnAmount = Mathf.Abs(angleDifference);
            facingReached = true;
        }
        turnAmount *= Mathf.Sign(angleDifference);
        rb.MoveRotation(transform.eulerAngles.z + turnAmount);
        return facingReached;
    }

    bool CanSeePlayer()
    {
        // Check if the zombie can see the player within its sight cone
        Vector2 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        directionToPlayer.Normalize();
        if (distanceToPlayer < minSightRange)
        {
            return true;
        }
        if (distanceToPlayer > maxSightRange)
        {
            return false;
        }

        float angleDifference = Vector2.Angle(transform.up, directionToPlayer);

        // Check if player is in the sight cone
        if (angleDifference > sightArc / 2f)
        {
            return false;
        }

        // Raycast to check if there are any obstacles blocking vision
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, blockVisionMask);
        return hit.collider == null;
    }

    public void Hurt(int amount)
    {
        // Handle getting hurt and transitioning states
        if (curState == States.dead) { return; }
        if (curState == States.idle)
        {
            SetAttackState();
        }
        lastKnownPlayerPosition = player.transform.position;
        hasLastKnownPosition = true;
        ZombieIsShot(true);
        health -= amount;
        bloodSprayer.SprayBlood();
        if (health <= 0)
        {
            deathSounds.PlaySound();
            SetDeadState();
        }
        else
        {
            hurtSounds.PlaySound();
        }
    }

    private void ZombieIsShot(bool isHurt)
    {
        // Handle the zombie being shot
        isShot = isHurt;
        StartCoroutine(ChangeZombieIsShotBool());
    }

    IEnumerator ChangeZombieIsShotBool()
    {
        // Reset the shot flag after a delay
        yield return new WaitForSeconds(2);
        isShot = false;
    }

    public void OnDrawGizmos()
    {
        // Draw gizmos for visual debugging
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + transform.up * attackDistance, attackDetectRadius);
        Gizmos.DrawWireSphere(transform.position + transform.up * attackDistance, attackRadius);
        Gizmos.DrawWireSphere(transform.position + transform.up * attackDistance, minSightRange);
        Gizmos.DrawWireSphere(transform.position + transform.up * attackDistance, maxSightRange);
    }

    //animation event todo here
    public void PlayStepSounds()
    {
        stepSounds.PlaySound();
    }
}
