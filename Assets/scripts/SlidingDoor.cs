using TMPro;
using UnityEditor.Tilemaps;
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    private Animator anim;
    [SerializeField] bool isLocked = false;
    [SerializeField] PlayRandomSound doorSound;
    [SerializeField] GameObject key;
    [SerializeField] TMP_Text lockedText;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(!isLocked)
            {
                anim.SetTrigger("Open");
                doorSound.PlaySound();
            }
            else if(isLocked && key.activeInHierarchy)
            {
                anim.SetTrigger("Open");
                doorSound.PlaySound();
                isLocked = true;
            }
            else
            {
                lockedText.gameObject.SetActive(true);
                return;
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            if(isLocked && !key.activeInHierarchy)
            {
                lockedText.gameObject.SetActive(false);
                return;
            }
            else
            {
                anim.SetTrigger("Close");
                doorSound.PlaySound();
            }

        }
    }
}
