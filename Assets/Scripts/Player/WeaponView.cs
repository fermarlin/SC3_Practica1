using UnityEngine;
using System.Collections;
using DG.Tweening;

public class WeaponView : MonoBehaviour
{
    [SerializeField] private GameObject muzzleFlash;

    private Renderer[] muzzleRenderers;
    private Coroutine muzzleRoutine;
    private Transform muzzleTransform;
    private Vector3 originalScale;
    private Quaternion originalRotation;

    private void Awake(){

        muzzleRenderers = muzzleFlash.GetComponentsInChildren<Renderer>(false);
        muzzleTransform = muzzleFlash.transform;

        originalScale = muzzleTransform.localScale;
        originalRotation = muzzleTransform.localRotation;
    }

    public void PlayShootEffect()
    {
        if (muzzleRoutine != null)
            StopCoroutine(muzzleRoutine);

        muzzleTransform.DOKill();

        muzzleRoutine = StartCoroutine(MuzzleFlashRoutine());
        
    }
    private void OnDestroy()
    {
        if (muzzleTransform != null)
            muzzleTransform.DOKill();
    }


    IEnumerator MuzzleFlashRoutine()
    {
        float randomAngle = Random.Range(0f, 360f);

        muzzleTransform.localRotation = originalRotation * Quaternion.Euler(0f, 0f, randomAngle);
        muzzleTransform.DOLocalRotate(
            muzzleTransform.localEulerAngles + new Vector3(0f, 0f, 90f),
            0.15f,
            RotateMode.FastBeyond360
        );

        muzzleTransform.localScale = originalScale * 1.8f;
        muzzleTransform.DOScale(originalScale * 0.5f, 0.15f).SetEase(Ease.OutQuad);

        foreach (Renderer renderer in muzzleRenderers)
        {
            renderer.enabled = true;
        }
        yield return new WaitForSeconds(.15f);

        foreach (Renderer renderer in muzzleRenderers)
        {
            renderer.enabled = false;
        }
        muzzleRoutine=null;
    }
}
