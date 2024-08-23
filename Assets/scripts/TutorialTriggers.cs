using DG.Tweening;
using TMPro;
using UnityEngine;

public class TutorialTriggers : MonoBehaviour
{
    [SerializeField] TMP_Text textToFadeOut;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            textToFadeOut.DOFade(0, 1f);
        }
    }

}
