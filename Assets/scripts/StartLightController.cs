using UnityEngine;
using UnityEngine.Rendering.Universal;

public class StartLightController : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] Light2D[] lights;

    [Header("Data")]
    [SerializeField] float timeToShutdownTheLights = 1.0f;
    [SerializeField] float lightDrainRate = 0.05f;
    private float timer;
    void Start()
    {
        timer = timeToShutdownTheLights;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            ShutOffTheLights();
        }
    }

    private void ShutOffTheLights()
    {
        foreach (Light2D light in lights)
        {
            if(light.intensity > 0)
            {
                light.intensity -= lightDrainRate * Time.deltaTime;
            }
        }
    }
}
