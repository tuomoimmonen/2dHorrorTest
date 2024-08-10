using UnityEngine;

public class BloodSprite : MonoBehaviour
{
    void Start()
    {
        int random = Random.Range(0, transform.childCount);
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i==random);
        }

        transform.GetChild(random).GetComponent<SpriteRenderer>().flipX = Random.Range(0,2) == 0;
    }

}
