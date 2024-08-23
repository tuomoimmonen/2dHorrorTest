using JetBrains.Annotations;
using System.Collections;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("ElementsInInventory")]
    [SerializeField] GameObject keyInInventory;
    [SerializeField] TMP_Text pickUpText;

    private void Awake()
    {
        keyInInventory.SetActive(false);
    }

    public void ActivateKeyObjectInInventory()
    {
        pickUpText.text = "Picked up a key";
        keyInInventory.SetActive(true);
        StartCoroutine(ClearPickUpText());
    }

    private IEnumerator ClearPickUpText()
    {
        yield return new WaitForSeconds(1);
        pickUpText.text = " ";
    }
}
