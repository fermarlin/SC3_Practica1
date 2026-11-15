
using UnityEngine;
using System.Collections;
using DG.Tweening;

//Este script se encarga de reproducir los efectos visuales del disparo del arma utilizo DOTween para animar el fogonazo y darle algo de variacion
public class WeaponView : MonoBehaviour
{
    //Objeto que contiene el efecto visual del disparo
    [SerializeField] private GameObject muzzleFlash;

    //Renderers que utilizo para mostrar y ocultar el fogonazo
    private Renderer[] muzzleRenderers;

    // Guard la corrutina para poder detenerla si disparo otra vez
    private Coroutine muzzleRoutine;

    //Transform del fogonazo para modificar su rotacion y escala
    private Transform muzzleTransform;

    //Guardo la escala y rotacion originales para poder restaurarlas en cada disparo
    private Vector3 originalScale;
    private Quaternion originalRotation;

    private void Awake()
    {
        // Pillo todos los renderers del fogonazo
        muzzleRenderers = muzzleFlash.GetComponentsInChildren<Renderer>(false);

        //Guardo su Transform
        muzzleTransform = muzzleFlash.transform;

        //Guardo sus valores originales
        originalScale = muzzleTransform.localScale;
        originalRotation = muzzleTransform.localRotation;
    }

    //Esta funcion se ejecuta cuando el jugador dispara
    public void PlayShootEffect()
    {
        //Si todavia se estaba reproduciendo el efecto anterior, detengo la corrutina
        if (muzzleRoutine != null)
        StopCoroutine(muzzleRoutine);

        //Detengo las animaciones anteriores de DOTween para que no se acumulen
        muzzleTransform.DOKill();

        //Inicio nuevamente el efecto de disparo
        muzzleRoutine = StartCoroutine(MuzzleFlashRoutine());
    }

    private void OnDestroy()
    {
        //Limpio las animaciones de DOTween cuando se destruye el arma
        if (muzzleTransform != null)
        muzzleTransform.DOKill();
    }

    //Esta corrutina se encarga de mostrar, animar y ocultar el fogonazo
    IEnumerator MuzzleFlashRoutine()
    {
        //Genero una rotacion aleatoria para que el efecto no sea siempre igual
        float randomAngle = Random.Range(0f, 360f);

        //Aplico la rotacion aleatoria tomando como referencia la rotacion original
        muzzleTransform.localRotation = originalRotation * Quaternion.Euler(0f, 0f, randomAngle);

        //Utilizo DOTween para animar un poco mas la rotacion del fogonazo
        muzzleTransform.DOLocalRotate(muzzleTransform.localEulerAngles + new Vector3(0f, 0f, 90f), 0.15f, RotateMode.FastBeyond360);

        //Aumento inicialmente el tamano del fogonazo
        muzzleTransform.localScale = originalScale * 1.8f;

        //Reduzco su tamano progresivamente utilizando DOTween
        muzzleTransform.DOScale(originalScale * 0.5f,0.15f).SetEase(Ease.OutQuad);

        //Activo los renderers para mostrar el fogonazo
        foreach (Renderer renderer in muzzleRenderers)
        {
            renderer.enabled = true;
        }

        //Mantengo el efecto visible durante un breve periodo
        yield return new WaitForSeconds(.15f);

        //Oculto nuevamente el fogonazo
        foreach (Renderer renderer in muzzleRenderers)
        {
            renderer.enabled = false;
        }

        //Limpio la referencia porque la corrutina ya ha terminado
        muzzleRoutine = null;
    }
}
