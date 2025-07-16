using System.Collections;
using UnityEngine;

public class SwordContainer : MonoBehaviour
{
    public GameObject swordPrefab;
    public WeaponProperties properties;

    public void Start()
    {
        GameObject sword = Instantiate(swordPrefab);
        sword.transform.SetParent(this.transform);
        sword.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public void UpdateQuality(WeaponProperties weaponProperties)
    {
        Debug.Log("Setting properties to: " + weaponProperties);
        this.properties = weaponProperties;
    }

}
