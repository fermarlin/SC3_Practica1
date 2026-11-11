
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider))]
public class AutomaticDoor : MonoBehaviour
{
    [SerializeField] private LayerMask detectionLayers;

    private Animator animator;
    private readonly HashSet<Collider> detected = new();

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((detectionLayers.value & (1 << other.gameObject.layer)) == 0)
            return;

        detected.Add(other);
        animator.SetBool("Open", true);
    }

    private void OnTriggerExit(Collider other)
    {
        detected.Remove(other);

        if (detected.Count == 0)
            animator.SetBool("Open", false);
    }

    private void OnDisable()
    {
        detected.Clear();
        if (animator != null)
            animator.SetBool("Open", false);
    }
}
