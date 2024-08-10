using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupTypes { health, pistolAmmo };
    public PickupTypes pickupTypes = PickupTypes.health; //default
    public int amount = 1;

}
