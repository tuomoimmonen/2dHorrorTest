using System;
using TMPro;
using UnityEngine;

public class GuardController : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] TMP_Text activateToInterractText;
    [SerializeField] GameObject textToSpeak;
    [SerializeField] PlayRandomSound sound;

    [Header("Data")]
    private bool isAlive;
    private bool playerNear;

    [Header("Events")]
    public static Action OnPlayerInterracting;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerNear)
        {
            playerNear = false;
            if (sound != null) { sound.PlaySound(); } //make sure guards have sounds too
            OnPlayerInterracting?.Invoke();
            textToSpeak.SetActive(true);
            activateToInterractText.text = " ";
            Time.timeScale = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            activateToInterractText.text = "Press [E] to interract";
            playerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            activateToInterractText.text = " ";
            playerNear = false;
        }
    }
}
