using System.Collections;
using UnityEngine;

public class BloodSprayer : MonoBehaviour
{
    [SerializeField] GameObject bloodSprite;
    [SerializeField] float maxSprayRange = 5f;
    [SerializeField] int mixBloodAmount = 1;
    [SerializeField] int maxBloodAmount = 2;

    [Header("Audio")]
    [SerializeField] PlayRandomSound splatterSounds;
    [SerializeField] float splatterSoundDelay = 0.3f;

    public void SprayBlood()
    {
        int randomBloodAmount = Random.Range(maxBloodAmount, mixBloodAmount+1);
        
        for (int i = 0; i < maxBloodAmount; i++)
        {
            float sprayRange = Random.Range(0f, maxSprayRange);
            float sprayAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector2 sprayDirection = new Vector2(Mathf.Cos(sprayAngle), Mathf.Sin(sprayAngle));
            Vector2 bloodPos = (Vector2)transform.position + sprayDirection * sprayRange;
            Instantiate(bloodSprite, bloodPos, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
        }

        StartCoroutine(PlaySounds());
    }

    IEnumerator PlaySounds()
    {
        yield return new WaitForSeconds(splatterSoundDelay);
        splatterSounds.PlaySound();
    }
}
