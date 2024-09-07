using UnityEngine;

public class JumpScareController : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] GameObject jumpScareToActivate;
    [SerializeField] Animator anim;
    [SerializeField] PlayRandomSound sound;

    [Header("Data")]
    [SerializeField] float timeBeforeDestroyJumpScare = 4f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jumpScareToActivate.SetActive(true);
            sound.PlaySound();
            anim.SetTrigger("scare");
            Invoke("DestroyJumpScareAfterDelay", timeBeforeDestroyJumpScare);
        }
    }

    private void DestroyJumpScareAfterDelay()
    {
        Destroy(gameObject);
    }




}
