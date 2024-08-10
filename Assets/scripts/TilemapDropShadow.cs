using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapDropShadow : MonoBehaviour
{
    Tilemap tilemap;
    TilemapRenderer tilemapRenderer;

    float alpha = 0.3f;
    [SerializeField] float offsetAmount = 0.4f;
    [SerializeField] float wallLightening = 0.1f;
    [SerializeField] float layerOffset = 0.4f;
    [SerializeField] int wallLayers = 4;
    Vector2 offsetDirection;

    private void Start()
    {
        GameObject duplicate = Instantiate(transform.parent.gameObject);
        duplicate.GetComponentInChildren<TilemapDropShadow>().enabled = false;

        tilemap = duplicate.GetComponent<Tilemap>();
        tilemapRenderer = duplicate.GetComponent<TilemapRenderer>();
        duplicate.GetComponent<TilemapCollider2D>().enabled = false;

        Color wallSideColor = Color.Lerp(tilemap.color, Color.white, wallLightening);
        Color wallTopColor = tilemap.color;
        transform.parent.GetComponent<Tilemap>().color = wallSideColor;

        Color shadowColor = Color.black;
        shadowColor.a = alpha;
        tilemap.color = shadowColor;
        tilemapRenderer.sortingOrder = -1;

        duplicate.transform.SetParent(transform.parent.transform.parent);
        float angle = DropShadow.offsetAngle * Mathf.Deg2Rad;
        offsetDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * offsetAmount;
        duplicate.transform.position = (Vector2)duplicate.transform.position + offsetDirection * offsetAmount;

        for (int i = 0; i < wallLayers; i++)
        {
            duplicate = Instantiate(transform.parent.gameObject);
            duplicate.GetComponentInChildren<TilemapDropShadow>().enabled = false;
            duplicate.GetComponent<TilemapCollider2D>().enabled = false;

            Color color = wallSideColor;
            if(i == wallLayers - 1)
            {
                color = wallTopColor;
            }
            duplicate.GetComponent<Tilemap>().color = color;
            duplicate.GetComponent<TilemapRenderer>().sortingOrder = 1 + i;
            duplicate.transform.position = (Vector2)duplicate.transform.position - offsetDirection * i * layerOffset;
            duplicate.transform.SetParent(transform.parent.transform.parent);
        }
    }

}
