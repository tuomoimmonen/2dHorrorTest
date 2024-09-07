using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlickeringLightController : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] Light2D lightObject;
    [SerializeField] PlayRandomSound sound;

    [Header("Data")]
    [SerializeField] float minTime;
    [SerializeField] float maxTime;
    [SerializeField] float minLightValue = 0.1f;
    [SerializeField] float maxLightValue = 0.7f;
    [SerializeField] float flickerTime = 1f;
    [SerializeField] float originalFlickerTime = 2f;

    [SerializeField] float tempTimer = 0.1f;
    private float timer;

    void Start()
    {
        timer = Random.Range(minTime, maxTime);
    }

    void Update()
    {
        FlickeringLight();
    }

    private void FlickeringLight()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            if (flickerTime > 0)
            {
                flickerTime -= Time.deltaTime;

                // Handle flickering within flicker time
                tempTimer -= Time.deltaTime;
                if (tempTimer <= 0)
                {
                    lightObject.intensity = Random.Range(minLightValue, maxLightValue);
                    sound.PlaySound();
                    tempTimer = 0.1f;  // Reset tempTimer for the next flicker
                }
            }
            else
            {
                // Reset flickerTime and timer after the flickering phase ends
                flickerTime = originalFlickerTime;
                timer = Random.Range(minTime, maxTime);
            }
        }
    }

}
