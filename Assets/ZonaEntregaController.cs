using UnityEngine;

public class ZonaEntregaController : MonoBehaviour
{
    public CajaController caja;

    public float radioEntrega = 1.5f;

    private bool entregada = false;

    public void ResetearEntrega()
    {
        entregada = false;
    }


    void Update()
    {
        if (!entregada && caja.EstaRecogida())
        {
            float distancia = Vector3.Distance(caja.transform.position, transform.position);
            if (distancia < radioEntrega)
            {
                Debug.Log("✅ ¡Caja entregada!");

                entregada = true;

            }
        }
    }

    public bool CajaFueEntregada()
    {
        return entregada;
    }
}
