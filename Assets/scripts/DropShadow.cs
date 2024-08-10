using UnityEngine;

public class DropShadow : MonoBehaviour
{
    SpriteRenderer sr;
    SpriteRenderer parentSr;

    [SerializeField] bool dynamic = true;
    [SerializeField] bool animated = false;

    public static float offsetAngle = -90f;
    [SerializeField] float offsetAmount = -0.2f;
    Vector2 offsetDirection;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        parentSr = transform.parent.GetComponent<SpriteRenderer>();
        sr.flipX = parentSr.flipX;
        sr.flipY = parentSr.flipY;
        sr.sprite = parentSr.sprite;
        sr.color = Color.black;
        CalculateOffsetDirection(offsetAmount);
    }

    void Update()
    {
        if (dynamic)
        {
            UpdateTransform();
        }
        if (animated)
        {
            sr.sprite = parentSr.sprite;
        }
    }

    public void CalculateOffsetDirection(float newOffset)
    {
        float angle = offsetAngle * Mathf.Deg2Rad;
        offsetDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle) * newOffset);
        UpdateTransform();
    }

    void UpdateTransform()
    {
        transform.position = (Vector2)parentSr.transform.position + offsetDirection;
        transform.rotation = parentSr.transform.rotation;
    }
}
