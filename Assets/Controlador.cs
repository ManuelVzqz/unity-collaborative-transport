using UnityEngine;
using Unity.MLAgents;

public class ControladorEpisodio : MonoBehaviour
{
    [Header("Referencias")]
    public AgenteColaborador[] agentes;
    public Transform caja;
    public ZonaEntregaController zonaEntregaController;
    private SimpleMultiAgentGroup m_AgentGroup;

    [Header("Posiciones iniciales")]
    private Vector3[] posicionesInicialesAgentes;
    private Quaternion[] rotacionesInicialesAgentes;
    private Vector3 posicionInicialCaja;
    private Quaternion rotacionInicialCaja;

    [Header("Parámetros")]
    public float alturaLimite = -1f;

    [Header("Tiempo máximo por episodio")]
    public float tiempoMaximo = 30f;
    private float tiempoTranscurrido = 0f;

    void Start()
    {
        Time.timeScale = 10f; 

        zonaEntregaController.ResetearEntrega(); 
        m_AgentGroup = new SimpleMultiAgentGroup();

        posicionesInicialesAgentes = new Vector3[agentes.Length];
        rotacionesInicialesAgentes = new Quaternion[agentes.Length];

        for (int i = 0; i < agentes.Length; i++)
        {
            posicionesInicialesAgentes[i] = agentes[i].transform.position;
            rotacionesInicialesAgentes[i] = agentes[i].transform.rotation;
            m_AgentGroup.RegisterAgent(agentes[i]);
            agentes[i].AsignarGrupo(m_AgentGroup);
        }

        posicionInicialCaja = caja.position;
        rotacionInicialCaja = caja.rotation;
    }

    void Update()
    {
        tiempoTranscurrido += Time.deltaTime;
        if (zonaEntregaController.CajaFueEntregada())
        {
            Debug.Log("🎯 ¡Caja entregada exitosamente! Reiniciando episodio. (Recompensa +100)");
            m_AgentGroup.AddGroupReward(100.0f); 
            TerminarEpisodio(true);
            return;
        }
        if (tiempoTranscurrido >= tiempoMaximo)
        {
            Debug.Log("⏱️ Tiempo máximo alcanzado. Reiniciando episodio.");
            m_AgentGroup.AddGroupReward(-0.5f);
            TerminarEpisodio(false); 
            return;
        }
        if (caja.position.y < alturaLimite)
        {
            Debug.Log("📦 Caja caída");
            m_AgentGroup.AddGroupReward(-1.0f);
            TerminarEpisodio(true);
            return;
        }
        foreach (var agente in agentes)
        {
            if (agente.transform.position.y < alturaLimite)
            {
                Debug.Log("🤕 Agente caído");
                m_AgentGroup.AddGroupReward(-1.0f);
                TerminarEpisodio(true);
                return;
            }
        }
    }

    void TerminarEpisodio(bool finalizoNaturalmente)
    {
        if (finalizoNaturalmente)
        {
            m_AgentGroup.EndGroupEpisode();
        }
        else
        {
            m_AgentGroup.GroupEpisodeInterrupted(); 
        }
        
        ResetearEscenaFisica();
    }

    void ResetearEscenaFisica()
    {
        tiempoTranscurrido = 0f;
        for (int i = 0; i < agentes.Length; i++)
        {
            agentes[i].transform.position = posicionesInicialesAgentes[i];
            agentes[i].transform.rotation = rotacionesInicialesAgentes[i];
            agentes[i].GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            agentes[i].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
        caja.position = posicionInicialCaja;
        caja.rotation = rotacionInicialCaja;
        Rigidbody rbCaja = caja.GetComponent<Rigidbody>();
        rbCaja.linearVelocity = Vector3.zero;
        rbCaja.angularVelocity = Vector3.zero;

        caja.GetComponent<CajaController>().SoltarCaja();
        zonaEntregaController.ResetearEntrega();
    }
}