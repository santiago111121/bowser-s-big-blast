using UnityEngine;
using System.Collections;
using System.Linq;

public class InteraccionAleatoria : MonoBehaviour
{
    public GameObject[] objetosInteractuables; // Los objetos interactuables.
    public Movimiento movimientoScript; // Referencia al script Movimiento.
    public SeguirCamino seguirCaminoScript; // Referencia al script SeguirCamino.
    public ParticleSystem particulasCorrectas1; // Partículas para la interacción correcta.
    public ParticleSystem particulasCorrectas2;
    public Light luzCorrecta; // Luz adicional que se activa con la interacción correcta.
    public GameObject rampa; // Rampa que baja cuando se interactúa con el objeto correcto.
    public GameObject bloqueo; // Objeto al que llega la rampa.
    public Light[] luces; // Arreglo de las 4 luces globales.

    [Header("Temporizadores")]
    public float tiempoAntesDeBajarRampa = 2f;
    public float tiempoRetrasoParticulas = 3f;
    public float tiempoEncendidaLuz = 1f;
    public float tiempoEncendidaLuces = 2f; // Tiempo que permanecen encendidas las luces.

    private int objetoCorrecto; // Índice del objeto correcto.
    private bool rampaEnMovimiento = false;
    private bool[] objetosDisponibles; // Array que indica si un objeto está disponible para interactuar.

    void Start()
    {
        objetosDisponibles = new bool[objetosInteractuables.Length];
        for (int i = 0; i < objetosDisponibles.Length; i++)
        {
            objetosDisponibles[i] = true;
        }

        if (particulasCorrectas1 != null)
            particulasCorrectas1.Stop();

        if (particulasCorrectas2 != null)
            particulasCorrectas2.Stop();

        if (luzCorrecta != null)
            luzCorrecta.enabled = false;

        ApagarTodasLasLuces();
        SeleccionarObjetoAleatorio();
    }

    void Update()
    {
        // Reiniciar el primer camino al presionar la barra espaciadora.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ReiniciarCamino();
        }
    }

    void ReiniciarCamino()
    {
        Debug.Log("Reiniciando el primer camino...");
        if (seguirCaminoScript != null)
        {
            seguirCaminoScript.ActivarPrimerCamino();
        }
        ResetearLucesYObjetos();
    }

    void ResetearLucesYObjetos()
    {
        ApagarTodasLasLuces();
        for (int i = 0; i < objetosInteractuables.Length; i++)
        {
            if (objetosInteractuables[i] != null)
            {
                objetosInteractuables[i].SetActive(true);
                objetosDisponibles[i] = true;
            }
        }
        SeleccionarObjetoAleatorio();
    }

    void SeleccionarObjetoAleatorio()
    {
        var indicesDisponibles = objetosInteractuables
            .Select((obj, index) => new { obj, index })
            .Where(o => objetosDisponibles[o.index])
            .Select(o => o.index)
            .ToArray();

        if (indicesDisponibles.Length > 0)
        {
            objetoCorrecto = indicesDisponibles[Random.Range(0, indicesDisponibles.Length)];
            Debug.Log($"El nuevo objeto correcto es: {objetosInteractuables[objetoCorrecto].name}");
        }
        else
        {
            Debug.Log("No quedan objetos disponibles.");
        }
    }

    public void InteractuarConObjeto(int indiceObjeto)
    {
        if (indiceObjeto >= objetosInteractuables.Length || !objetosDisponibles[indiceObjeto])
        {
            Debug.Log("El objeto ya no está disponible para interacción.");
            return;
        }

        if (indiceObjeto == objetoCorrecto)
        {
            Debug.Log("¡Interacción correcta!");
            EncenderLucesGlobales(Color.red);
            if (movimientoScript != null)
                movimientoScript.ReiniciarMovimiento();

            StartCoroutine(ActivarLuzYParticulasConRetraso());
            StartCoroutine(BajarRampaConEliminacion(objetosInteractuables[indiceObjeto]));

            SeleccionarObjetoAleatorioRestante(indiceObjeto);
        }
        else
        {
            Debug.Log("No es el objeto correcto. Intenta con otro.");
            EncenderLucesGlobales(Color.green);
            StartCoroutine(EsperarYActivarCaminoIncorrecto());
        }
    }

    IEnumerator EsperarYActivarCaminoIncorrecto()
    {
        yield return new WaitForSeconds(2f);
        if (seguirCaminoScript != null)
            seguirCaminoScript.ActivarCaminoIncorrecto();
    }

    IEnumerator ActivarLuzYParticulasConRetraso()
    {
        yield return new WaitForSeconds(tiempoRetrasoParticulas);

        if (luzCorrecta != null)
        {
            luzCorrecta.enabled = true;
            StartCoroutine(ApagarLuzDespuesDeTiempo());
        }

        if (particulasCorrectas1 != null)
            particulasCorrectas1.Play();

        if (particulasCorrectas2 != null)
            particulasCorrectas2.Play();
    }

    IEnumerator ApagarLuzDespuesDeTiempo()
    {
        yield return new WaitForSeconds(tiempoEncendidaLuz);
        if (luzCorrecta != null)
            luzCorrecta.enabled = false;
    }

    IEnumerator BajarRampaConEliminacion(GameObject objetoCorrecto)
    {
        rampaEnMovimiento = true;
        yield return new WaitForSeconds(tiempoAntesDeBajarRampa);

        Vector3 posicionInicial = rampa.transform.position;
        Vector3 posicionFinal = bloqueo.transform.position;

        float duracion = 2f;
        float tiempo = 0;

        while (tiempo < duracion)
        {
            rampa.transform.position = Vector3.Lerp(posicionInicial, posicionFinal, tiempo / duracion);
            tiempo += Time.deltaTime;
            yield return null;
        }

        rampa.transform.position = posicionFinal;

        EliminarObjetoCorrecto(objetoCorrecto);

        yield return new WaitForSeconds(2f);

        StartCoroutine(SubirRampa(posicionInicial));
    }

    void EliminarObjetoCorrecto(GameObject objetoCorrecto)
    {
        objetosDisponibles[System.Array.IndexOf(objetosInteractuables, objetoCorrecto)] = false;
        Destroy(objetoCorrecto);
    }

    IEnumerator SubirRampa(Vector3 posicionInicial)
    {
        Vector3 posicionFinal = rampa.transform.position;

        float duracion = 2f;
        float tiempo = 0;

        while (tiempo < duracion)
        {
            rampa.transform.position = Vector3.Lerp(posicionFinal, posicionInicial, tiempo / duracion);
            tiempo += Time.deltaTime;
            yield return null;
        }

        rampa.transform.position = posicionInicial;
        rampaEnMovimiento = false;
    }

    void EncenderLucesGlobales(Color color)
    {
        foreach (var luz in luces)
        {
            if (luz != null)
            {
                luz.color = color;
                luz.enabled = true;
            }
        }
        StartCoroutine(ApagarLucesGlobales());
    }

    IEnumerator ApagarLucesGlobales()
    {
        yield return new WaitForSeconds(tiempoEncendidaLuces);
        ApagarTodasLasLuces();
    }

    void ApagarTodasLasLuces()
    {
        foreach (var luz in luces)
        {
            if (luz != null)
                luz.enabled = false;
        }
    }

    void SeleccionarObjetoAleatorioRestante(int indiceEliminado)
    {
        objetosDisponibles[indiceEliminado] = false;
        SeleccionarObjetoAleatorio();
    }
}
