using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Elements")]
    Rigidbody2D rb;
    [SerializeField] Camera cam;
    [SerializeField] Transform frontWheelR;
    [SerializeField] Transform frontWheelL;
    [SerializeField] Transform handR;
    [SerializeField] Transform handL;
    [SerializeField] Transform streerinWheelL;
    [SerializeField] Transform streerinWheelR;
    [SerializeField] Transform gunBase;
    [SerializeField] GunManager gunManager;
    [SerializeField] BloodSprayer bloodSprayer;
    [SerializeField] TMP_Text healthText;
    [SerializeField] Animator deathScreenAnim;
    [SerializeField] ScreenFader screenFader;

    public float acceleration = 2f;
    public float brakeForce = 0.5f;
    public float turnSpeed = 50f;

    [Header("Data")]
    [SerializeField] float maxSpeed = 1f;
    [SerializeField] float timeToMaxSpeed = 1f;
    [SerializeField] float moveDrag = 0.01f;
    [SerializeField] float brakeDrag = 0.08f;
    [SerializeField] float maxTurn = 60f;
    [SerializeField] float timeToMaxTurn = 0.3f;
    [SerializeField] int maxHealth = 5;
    int health;
    public bool isDead = false;
    float curTurn = 0f;
    float curSpeed = 0f;

    [Header("Audio")]
    [SerializeField] AudioSource engineSound;
    [SerializeField] float maxEnginePitch = 1.5f;
    [SerializeField] PlayRandomSound hurtSounds;
    [SerializeField] PlayRandomSound deathSounds;
    [SerializeField] PlayRandomSound healSounds;
    [SerializeField] PlayRandomSound pickupSounds;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = maxHealth;
    }

    private void Start()
    {
        UpdateHealthDisplay();
    }


    void Update()
    {
        if(isDead) { return; }

        cam.transform.rotation = Quaternion.Euler(0f,0f,0f);
        handL.rotation = gunBase.rotation;
        handL.position = gunBase.position;

        float t = 0.5f + (curTurn / maxTurn) / 2f;
        handR.position = Vector2.Lerp(streerinWheelR.position, streerinWheelL.position, t);

        if (Input.GetButtonDown("Fire1"))
        {
            gunManager.Shoot();
        }
    }

    private void FixedUpdate()
    {
        if(isDead) 
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            return; 
        }
        
        float gasInput = Input.GetAxisRaw("Vertical");
        curSpeed += gasInput * (maxSpeed / timeToMaxSpeed);
        curSpeed = Mathf.Clamp(curSpeed, -maxSpeed, maxSpeed);

        //brakes
        if (Input.GetButton("Jump"))
        {
            curSpeed = Mathf.Lerp(curSpeed, 0f, brakeDrag); //brake drag when pressing brake
        }
        else if (Mathf.Approximately(gasInput, 0f))
        {
            curSpeed = Mathf.Lerp(curSpeed, 0f, moveDrag); //movedrag when gasInput
        }

        float steeringInput = -Input.GetAxisRaw("Horizontal");
        bool returnSteerToZero = Mathf.Approximately(steeringInput, 0f);
        if (returnSteerToZero && !Mathf.Approximately(curTurn, 0f)) 
        {
            steeringInput = -Mathf.Sign(curTurn);
        }
        curTurn += steeringInput * (maxSpeed / timeToMaxSpeed);
        curTurn = Mathf.Clamp(curTurn, -maxTurn, maxTurn);

        if(returnSteerToZero && Mathf.Sign(curTurn) == Mathf.Sign(steeringInput))
        {
            curTurn = 0f;
        }

        frontWheelL.localRotation = Quaternion.Euler(0f, 0f, curTurn);
        frontWheelR.localRotation = Quaternion.Euler(0f,0f, curTurn);

        rb.velocity = rb.transform.up * curSpeed;
        rb.angularVelocity = curTurn * (curSpeed / maxSpeed);

        gunManager.UpdateAmmoDisplayPosition();
        engineSound.pitch = Mathf.Lerp(1.0f, maxEnginePitch, Mathf.Abs(curSpeed / maxSpeed));
        /*
        // Get input
        float gasInput = Input.GetAxisRaw("Vertical");
        float steeringInput = -Input.GetAxisRaw("Horizontal");

        // Handle acceleration and braking
        if (gasInput > 0)
        {
            curSpeed += gasInput * acceleration;
        }
        else if (gasInput <= 0)
        {
            curSpeed += gasInput * brakeForce;
        }

        if (Input.GetButton("Jump"))
        {
            curSpeed = Mathf.Lerp(curSpeed, 0f, brakeDrag); //brake drag when pressing brake
        }
        else
        {
            curSpeed = Mathf.Lerp(curSpeed, 0f, moveDrag);
        }

        // Clamp the speed to maxSpeed
        curSpeed = Mathf.Clamp(curSpeed, -maxSpeed, maxSpeed);

        // Handle steering
        curTurn = steeringInput * turnSpeed;

        // Apply the movement and rotation
        rb.velocity = transform.up * curSpeed;
        rb.angularVelocity = curTurn * (curSpeed / maxSpeed);

        frontWheelL.localRotation = Quaternion.Euler(0f, 0f, curTurn);
        frontWheelR.localRotation = Quaternion.Euler(0f, 0f, curTurn);
        */
    }

    public void Hurt(int damage)
    {
        if(isDead)
        {
            engineSound.Stop();
            return;
        }
        health -= damage;
        bloodSprayer.SprayBlood();
        hurtSounds.PlaySound();

        if(health <= 0f)
        {
            deathSounds.PlaySound();
            isDead = true;
            gunManager.SetDead();
            deathScreenAnim.SetTrigger("FadeIn");
            //Debug.Log("Dead");
        }
        UpdateHealthDisplay();
    }

    void Heal(int amount)
    {
        health += amount;
        if(health > maxHealth)
        {
            health = maxHealth;
        }
        UpdateHealthDisplay();
    }

    void UpdateHealthDisplay()
    {
        healthText.text = "health: " + health.ToString() + "/" + maxHealth.ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Pickup>() != null)
        {
            Pickup pickup = collision.GetComponent<Pickup>();
            if(pickup.pickupTypes == Pickup.PickupTypes.health && health < maxHealth)
            {
                healSounds.PlaySound();
                Heal(pickup.amount);
                Destroy(pickup.gameObject);
            }

            if(pickup.pickupTypes == Pickup.PickupTypes.pistolAmmo)
            {
                pickupSounds.PlaySound();
                gunManager.AddAmmo(pickup.amount);
                Destroy(pickup.gameObject);
            }
        }

        if(collision.CompareTag("Exit"))
        {
            screenFader.LoadNextLevel();
        }

        if (collision.CompareTag("SoundTrigger"))
        {
            collision.GetComponent<AudioSource>().Play();
            collision.GetComponent<Collider2D>().enabled = false;
        }
    }
}
