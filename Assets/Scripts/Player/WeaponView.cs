using UnityEngine;

public class WeaponView : MonoBehaviour
{
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private GameObject muzzleFlash;

    public void PlayShootEffect()
    {
        GameObject muzzle = Instantiate(muzzleFlash, muzzlePoint);
        Destroy(muzzle,5f);
    }
}
