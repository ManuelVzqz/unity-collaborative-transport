using UnityEngine;

public class ControlAnimaciones : MonoBehaviour
{
    private Animator animator;
    private Vector3 posicionAnterior;
    private float velocidad;

    void Start()
    {
        animator = GetComponent<Animator>();
        posicionAnterior = transform.position;
    }

    void Update()
    {
        velocidad = (transform.position - posicionAnterior).magnitude / Time.deltaTime;
        posicionAnterior = transform.position;
        if (animator != null)
            animator.SetFloat("Speed", velocidad);
    }
}