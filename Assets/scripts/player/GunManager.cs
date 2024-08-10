using TMPro;
using UnityEngine;

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
    [SerializeField] GameObject bulletSparks;
    [SerializeField] GameObject bloodSmoke;
    [SerializeField] TMP_Text ammoText;
    bool isDead;

    private void Start()
    {
        UpdateAmmoDisplay();
    }
    void Update()
    {
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
        if(pistolAmmo <= 0) { return; }
        pistolAmmo--;
        UpdateAmmoDisplay();
        Instantiate(gunSmoke, gunBase.transform.position, Quaternion.identity);

        flash.StartFlash();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 1000f, shootableMask);
        if(hit.collider != null)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                ZombieController zombie = hit.collider.GetComponent<ZombieController>();
                if(zombie != null)
                {
                    Instantiate(gunSmoke, hit.point, Quaternion.identity);
                    zombie.Hurt(damage);
                }
            }
            else
            {
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
        isDead = true;
        ammoText.gameObject.SetActive(false);
    }

    public void AddAmmo(int amount)
    {
        pistolAmmo += amount;
        UpdateAmmoDisplay();
    }


}
