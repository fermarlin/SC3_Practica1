
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField, Min(0.1f)]
    private float interactionRadius = 2.5f;

    [SerializeField]
    private LayerMask interactionLayers = ~0;

    private PlayerInputs inputs;

    private void Awake()
    {
        inputs = new PlayerInputs();
    }

    private void OnEnable()
    {
        inputs.PlayerActionMap.Repair.Enable();
    }

    private void OnDisable()
    {
        inputs.PlayerActionMap.Repair.Disable();
    }

    private void OnDestroy()
    {
        inputs.Dispose();
    }

    private void Update()
    {
        if (!inputs.PlayerActionMap.Repair.IsPressed())
            return;

        IInteractable target = FindInteractable();

        if (target != null)
            target.Interact(Time.deltaTime);
    }

    private IInteractable FindInteractable()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            interactionRadius,
            interactionLayers,
            QueryTriggerInteraction.Collide
        );

        IInteractable closest = null;
        float closestDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            if (hit.transform.IsChildOf(transform))
                continue;

            IInteractable interactable =
                hit.GetComponentInParent<IInteractable>();

            if (interactable == null)
                continue;

            if (interactable is Behaviour behaviour &&
                !behaviour.isActiveAndEnabled)
                continue;

            Vector3 point =
                hit.ClosestPoint(transform.position);

            float distance =
                (point - transform.position).sqrMagnitude;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = interactable;
            }
        }

        return closest;
    }
}
