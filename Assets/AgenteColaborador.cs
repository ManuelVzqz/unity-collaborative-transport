using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgenteColaborador : Agent
{
    [Header("Movimiento")]
    public float velocidadMovimiento = 5f;
    private Rigidbody rb;

    [Header("Referencias externas")]
    public Transform caja;
    public Transform otroAgente;
    public CajaController cajaController;
    public Transform zonaEntrega;

    [Header("Normalización de Recompensas")]
    [Tooltip("Escala las recompensas para alinear POCA (~150) con PPO/SAC sin alterar el comportamiento.")]
    public float factorNormalizacion = 0.75f;

    private bool estabaRecogidaAntes = false;
    private bool recompensaCoordOtorgada = false;
    private bool recompensaCercaZonaEntregada = false;
    private bool recompensaRecogerCajaOtorgada = false;
    private SimpleMultiAgentGroup miGrupo;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void AsignarGrupo(SimpleMultiAgentGroup grupo)
    {
        miGrupo = grupo;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.InverseTransformDirection(rb.linearVelocity)); 
        float distanciaCaja = Vector3.Distance(transform.position, caja.position);
        sensor.AddObservation(distanciaCaja < 2f ? 1f : 0f);                         
        sensor.AddObservation((distanciaCaja >= 2f && distanciaCaja < 5f) ? 1f : 0f);
        sensor.AddObservation(distanciaCaja >= 5f ? 1f : 0f);
        float distanciaZona = Vector3.Distance(transform.position, zonaEntrega.position);
        sensor.AddObservation(distanciaZona < 4f ? 1f : 0f);                         
        sensor.AddObservation((distanciaZona >= 4f && distanciaZona < 8f) ? 1f : 0f);
        sensor.AddObservation(distanciaZona >= 8f ? 1f : 0f);
        sensor.AddObservation(cajaController.EstaRecogida() ? 1f : 0f);              
        Vector3 posCajaRelativa = transform.InverseTransformPoint(caja.position);
        sensor.AddObservation(posCajaRelativa.x);                                    
        sensor.AddObservation(posCajaRelativa.z);
        Vector3 posOtroRelativa = transform.InverseTransformPoint(otroAgente.position);
        sensor.AddObservation(posOtroRelativa.x);                                    
        sensor.AddObservation(posOtroRelativa.z);
        Vector3 direccionZona = zonaEntrega.position - transform.position;
        float angulo = Vector3.Angle(transform.forward, direccionZona);
        sensor.AddObservation(angulo < 30f ? 1f : 0f);                               
    }

    public override void OnEpisodeBegin()
    {
        recompensaCercaZonaEntregada = false;
        recompensaCoordOtorgada = false;
        estabaRecogidaAntes = false;
        recompensaRecogerCajaOtorgada = false;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float movX = actions.ContinuousActions[0];
        float movZ = actions.ContinuousActions[1];
        Vector3 direccion = new Vector3(movX, 0, movZ);
        rb.AddForce(direccion * velocidadMovimiento, ForceMode.VelocityChange);

        float distanciaCaja = Vector3.Distance(transform.position, caja.position);
        float distanciaOtroAgente = Vector3.Distance(otroAgente.position, caja.position);
        float distanciaEntreAgentes = Vector3.Distance(transform.position, otroAgente.position);
        AddReward(-0.001f * distanciaCaja * factorNormalizacion);
        AddReward(-0.001f * distanciaEntreAgentes * factorNormalizacion);
        if (!recompensaCoordOtorgada &&
            distanciaCaja < 2f &&
            distanciaOtroAgente < 2f &&
            distanciaEntreAgentes < 3f &&
            !cajaController.EstaRecogida())
        {
            miGrupo?.AddGroupReward(1.0f * factorNormalizacion);
            recompensaCoordOtorgada = true;
        }

        if (!recompensaRecogerCajaOtorgada && !estabaRecogidaAntes && cajaController.EstaRecogida())
        {
            Debug.Log($"✅ Caja recogida (+{50.0f * factorNormalizacion})");
            miGrupo?.AddGroupReward(50.0f * factorNormalizacion);
            recompensaRecogerCajaOtorgada = true;
        }

        if (estabaRecogidaAntes && !cajaController.EstaRecogida())
        {
            Debug.Log($"❌ ¡Se cayó la caja! (-{20.0f * factorNormalizacion})");
            miGrupo?.AddGroupReward(-20.0f * factorNormalizacion);
        }

        estabaRecogidaAntes = cajaController.EstaRecogida();

        if (cajaController.EstaRecogida())
        {
            float distanciaZona = Vector3.Distance(caja.position, zonaEntrega.position);
            miGrupo?.AddGroupReward(-0.0005f * distanciaZona * factorNormalizacion); 

            if (!recompensaCercaZonaEntregada && distanciaZona < 8f)
            {
                float recompensa = distanciaZona < 5f ? 5.0f : 2.0f;
                miGrupo?.AddGroupReward(recompensa * factorNormalizacion);
                recompensaCercaZonaEntregada = true;
                Debug.Log($"📍 Recompensa por acercarse a la zona de entrega (+{recompensa * factorNormalizacion})");
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var acciones = actionsOut.ContinuousActions;
        acciones[0] = Input.GetAxis("Horizontal");
        acciones[1] = Input.GetAxis("Vertical");
    }
}