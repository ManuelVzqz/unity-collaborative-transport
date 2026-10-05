using UnityEngine;

public class CajaController : MonoBehaviour
{
    public Transform agente1;
    public Transform agente2;

    public float distanciaMaxCajaAgente = 3f;
    public float distanciaMaxEntreAgentes = 3.0f;

    private bool recogida = false;
    private Vector3 posicionInicial;

    public float velocidadSeguimiento = 5f;

    private Rigidbody rb;

    void Start()
    {
        posicionInicial = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float d1 = Vector3.Distance(transform.position, agente1.position);
        float d2 = Vector3.Distance(transform.position, agente2.position);
        float distanciaEntreAgentes = Vector3.Distance(agente1.position, agente2.position);

        if (!recogida)
        {
            if (d1 < distanciaMaxCajaAgente && d2 < distanciaMaxCajaAgente && distanciaEntreAgentes < distanciaMaxEntreAgentes)
            {
                recogida = true;
                Debug.Log("✅ Caja recogida por ambos agentes.");
                CambiarFisicaCaja(false); 
            }
        }
        else
        {
            if (d1 > distanciaMaxCajaAgente * 1.5f || d2 > distanciaMaxCajaAgente * 1.5f || distanciaEntreAgentes > distanciaMaxEntreAgentes * 1.5f)
            {
                recogida = false;
                Debug.Log("⚠️ La caja se soltó, los agentes se alejaron.");
                CambiarFisicaCaja(true);  
            }
            else
            {
                Vector3 puntoMedio = (agente1.position + agente2.position) / 2f;
                puntoMedio.y += 0.75f;

                transform.position = Vector3.Lerp(transform.position, puntoMedio, Time.deltaTime * velocidadSeguimiento);
            }
        }
    }

    private void CambiarFisicaCaja(bool activarFisica)
    {
        rb.useGravity = activarFisica;
        rb.isKinematic = !activarFisica;
    }

    public bool EstaRecogida()
    {
        return recogida;
    }

    public void SoltarCaja()
    {
        recogida = false;
        CambiarFisicaCaja(true);
        GetComponent<Collider>().enabled = true;
    }
}
