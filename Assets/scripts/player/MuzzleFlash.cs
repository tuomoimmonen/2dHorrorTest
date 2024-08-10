using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    SpriteRenderer sr;
    [SerializeField] float flashTime = 0.05f;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        EndFlash();
    }

    public void StartFlash()
    {
        sr.enabled = true;
        sr.flipX = true;
        Invoke("EndFlash", flashTime);
    }

    public void EndFlash()
    {
        sr.enabled = false;
        sr.flipX = false;
    }
}
