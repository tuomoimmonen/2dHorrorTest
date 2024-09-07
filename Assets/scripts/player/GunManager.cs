using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GunManager : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] Transform gunBase;
    [SerializeField] float maxDistance;
    [SerializeField] int damage;
    [SerializeField] int pistolAmmo;
    [SerializeField] LayerMask shootableMask;
    [SerializeField] MuzzleFlash flash;
    [SerializeField] GameObject gunSmoke;
    //[SerializeField] GameObject emptyShell;
    //[SerializeField] Transform shellStartPos;
    [SerializeField] ParticleSystem emptyShells;
    [SerializeField] GameObject bulletSparks;
    [SerializeField] GameObject bloodSmoke;
    [SerializeField] TMP_Text ammoText;
    bool isDead;
    private bool canMove;

    [Header("Audio")]
    [SerializeField] PlayRandomSound gunSounds;
    [SerializeField] PlayRandomSound emptyGunSounds;

    private void Awake()
    {
    }
    private void Start()
    {
        LoadData();
        canMove = true;
        UpdateAmmoDisplay();
        GuardController.OnPlayerInterracting += OnPlayerTalkingCallback;
        PlayerController.OnPlayerFinishedInterract += OnPlayerFinishedInterractCallback;
        ScreenFader.OnLevelFinished += OnLevelFinishedCallback;
        PauseMenuController.OnGameUnPaused += OnPlayerFinishedInterractCallback;
        PauseMenuController.OnGamePaused += OnPlayerTalkingCallback;
    }

    private void OnDestroy()
    {
        GuardController.OnPlayerInterracting -= OnPlayerTalkingCallback;
        PlayerController.OnPlayerFinishedInterract -= OnPlayerFinishedInterractCallback;
        ScreenFader.OnLevelFinished -= OnLevelFinishedCallback;
        PauseMenuController.OnGamePaused -= OnPlayerTalkingCallback;
        PauseMenuController.OnGameUnPaused -= OnPlayerFinishedInterractCallback;
    }
    private void OnLevelFinishedCallback()
    {
        SaveData();
    }
    private void OnPlayerFinishedInterractCallback()
    {
        canMove = true;
    }

    private void OnPlayerTalkingCallback()
    {
        canMove = false;
    }

    void Update()
    {
        if (isDead || !canMove) {  return; }
        FaceMouse();
    }

    void FaceMouse()
    {
        Vector3 worldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseDirection = new Vector2(worldPosition.x, worldPosition.y) - (Vector2)transform.position;
        transform.up = mouseDirection;

        float yPos = mouseDirection.magnitude;

        if(yPos > maxDistance) { yPos = maxDistance; }
        gunBase.localPosition = new Vector2(0f, yPos);
    }

    public void Shoot()
    {
        if(pistolAmmo <= 0) 
        {
            emptyGunSounds.PlaySound();
            return; 
        }
        emptyShells.Play();
        gunSounds.PlaySound();
        pistolAmmo--;
        UpdateAmmoDisplay();
        Instantiate(gunSmoke, gunBase.transform.position, Quaternion.identity);
        //Instantiate(emptyShell, shellStartPos.position, Quaternion.identity);

        flash.StartFlash();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 1000f, shootableMask);
        if(hit.collider != null)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                //Debug.Log(hit.collider.name);
                ZombieController zombie = hit.collider.GetComponent<ZombieController>();
                if(zombie != null)
                {
                    Instantiate(gunSmoke, hit.point, Quaternion.identity);
                    zombie.Hurt(damage);
                }
            }
            else
            {
                //Debug.Log(hit.collider.name);
                Instantiate(gunSmoke, hit.point, Quaternion.identity);
                Instantiate(bulletSparks, hit.point, Quaternion.identity);
            }
        }
    }

    public void UpdateAmmoDisplayPosition()
    {
        ammoText.rectTransform.position = Input.mousePosition;
    }

    void UpdateAmmoDisplay()
    {
        ammoText.text = pistolAmmo.ToString();
    }

    public void SetDead()
    {
        PlayerPrefs.SetInt("ammo", 6);
        isDead = true;
        ammoText.gameObject.SetActive(false);
    }

    public void AddAmmo(int amount)
    {
        pistolAmmo += amount;
        UpdateAmmoDisplay();
    }

    private void LoadData()
    {
        if(SceneManager.GetActiveScene().buildIndex == 3 || SceneManager.GetActiveScene().buildIndex == 2) { return; } //reset pistol ammo at tutorial and level1

        pistolAmmo = PlayerPrefs.GetInt("ammo", 6);
        //StartCoroutine(LoadDataAfterFrame());
    }

    private IEnumerator LoadDataAfterFrame()
    {
        yield return new WaitForEndOfFrame();
        pistolAmmo = PlayerPrefs.GetInt("ammo", 6);
    }

    private void SaveData()
    {
        if(isDead) { return; }
        PlayerPrefs.SetInt("ammo", pistolAmmo);
    }

}
