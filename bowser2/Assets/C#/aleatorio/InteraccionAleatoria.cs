using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class InteraccionAleatoria : MonoBehaviour
{

    public GameObject panelFinJuego; // Panel que aparecerá cuando se detecte el objeto correcto
    public Button botonSalir; // Botón para salir al menú
    public Button botonReiniciar; // Botón para reiniciar el juego

    public GameObject[] objetosInteractuables;
    public Movimiento movimientoScript;
    public SeguirCamino seguirCaminoScript;
    public ParticleSystem particulasCorrectas1;
    public ParticleSystem particulasCorrectas2;
    public Light luzCorrecta;
    public GameObject rampa;
    public GameObject bloqueo;
    public Light[] luces;
    public GameObject[] jugadores; // Referencia a los objetos de los jugadores (Jugador 1, 2, 3, 4)

    [Header("Temporizadores")]
    public float tiempoAntesDeBajarRampa = 2f;
    public float tiempoRetrasoParticulas = 3f;
    public float tiempoEncendidaLuz = 1f;
    public float tiempoEncendidaLuces = 2f;

    [Header("Sonidos")]
    public AudioSource audioVerde;
    public AudioSource audioRojo;
    public AudioSource audioDesaparicion;
    public AudioSource audioDesaparicion2;
    public AudioSource audioRampa; // Sonido de la rampa al moverse

    private int objetoCorrecto;
    private bool rampaEnMovimiento = false;
    private bool[] objetosDisponibles;
    private int jugadorActual = 0; // Controla el jugador que puede interactuar



    // Mostrar el panel de fin de juego cuando se detecte el objeto correcto
    void MostrarPanelFinJuego()

    {

        Time.timeScale = 0f; // Pausar el juego
        panelFinJuego.SetActive(true); // Mostrar el panel

    }


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

        // Asegurar que todos los jugadores estén visibles pero desactivar sus componentes excepto el modelo
        foreach (GameObject jugador in jugadores)
        {
            if (jugador != null)
            {
                jugador.SetActive(true);
                DesactivarComponentesJugador(jugador);
            }
        }

        // Activar al jugador 1
        if (jugadores.Length > 0 && jugadores[0] != null)
        {
            ActivarComponentesJugador(jugadores[0]);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ReiniciarCamino();
        }
    }

    void ReiniciarCamino()
    {
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
            Debug.Log("Objeto correcto actual: " + objetosInteractuables[objetoCorrecto].name);
        }
    }

    public void InteractuarConObjeto(int indiceObjeto)
    {
        if (indiceObjeto >= objetosInteractuables.Length || !objetosDisponibles[indiceObjeto])
        {
            return;
        }

        if (indiceObjeto == objetoCorrecto)
        {
            EncenderLucesGlobales(Color.red);
            if (movimientoScript != null)
                movimientoScript.ReiniciarMovimiento();

            StartCoroutine(ActivarLuzYParticulasConRetraso());
            StartCoroutine(BajarRampaConEliminacion(objetosInteractuables[indiceObjeto]));

            // Hacer desaparecer al jugador actual
            DesaparecerJugador(jugadores[jugadores.Length - 1]);

            // Mostrar el panel y los botones
            MostrarPanelFinJuego();

            StartCoroutine(MostrarPanelConRetraso());
        }
        else
        {
            EncenderLucesGlobales(Color.green);
            StartCoroutine(EsperarYActivarCaminoIncorrecto());
        }

        SeleccionarObjetoAleatorioRestante(indiceObjeto);

        // Activar al siguiente jugador después de cualquier interacción
        ActivarJugadorSiguiente(jugadorActual + 1);
    }

    void ActivarJugadorSiguiente(int jugadorIndex)
    {
        StartCoroutine(ActivarJugadorConRetraso(jugadorIndex));
    }

    IEnumerator ActivarJugadorConRetraso(int jugadorIndex)
    {
        yield return new WaitForSeconds(4.5f); // Espera 5 segundos

        // Activar los componentes del siguiente jugador
        if (jugadorIndex < jugadores.Length && jugadores[jugadorIndex] != null)
        {
            ActivarComponentesJugador(jugadores[jugadorIndex]);
            jugadorActual = jugadorIndex;
        }
        else
        {
            Debug.Log("Todos los jugadores han completado su turno.");
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

        if (audioDesaparicion != null)
            audioDesaparicion.Play();

        if (audioDesaparicion2 != null)
            audioDesaparicion2.Play();
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

        audioRampa.Play();

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

        audioRampa.Stop();
        EliminarObjetoCorrecto(objetoCorrecto);
        yield return new WaitForSeconds(2f);

        StartCoroutine(SubirRampa(posicionInicial));
    }

    IEnumerator SubirRampa(Vector3 posicionInicial)
    {
        audioRampa.Play();

        Vector3 posicionFinal = rampa.transform.position;
        float duracion = 2f;
        float tiempo = 0;

        while (tiempo < duracion)
        {
            rampa.transform.position = Vector3.Lerp(posicionFinal, posicionInicial, tiempo / duracion);
            tiempo += Time.deltaTime;
            yield return null;
        }

        audioRampa.Stop();
        rampa.transform.position = posicionInicial;
        rampaEnMovimiento = false;
    }

    void EliminarObjetoCorrecto(GameObject objetoCorrecto)
    {
        objetosDisponibles[System.Array.IndexOf(objetosInteractuables, objetoCorrecto)] = false;
        Destroy(objetoCorrecto);
    }

    void EncenderLucesGlobales(Color color)
    {
        StartCoroutine(EncenderLucesConRetraso(color));
    }

    IEnumerator EncenderLucesConRetraso(Color color)
    {
        yield return new WaitForSeconds(2f);
        if (color == Color.green)
            audioVerde.Play();
        else if (color == Color.red)
            audioRojo.Play();

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

    void DesactivarComponentesJugador(GameObject jugador)
    {
        var movimiento = jugador.GetComponent<Movimiento>();
        if (movimiento != null)
        {
            movimiento.enabled = false;
        }
        var otrosComponentes = jugador.GetComponents<Component>().Where(c => !(c is Transform || c is Renderer || c is GameObject));
        foreach (var componente in otrosComponentes)
        {
            if (componente is MonoBehaviour monoComponente)
            {
                monoComponente.enabled = false;
            }
        }
    }

    void ActivarComponentesJugador(GameObject jugador)
    {
        var movimiento = jugador.GetComponent<Movimiento>();
        if (movimiento != null)
        {
            movimiento.enabled = true;
        }
        var otrosComponentes = jugador.GetComponents<Component>().Where(c => !(c is Transform || c is Renderer || c is GameObject));
        foreach (var componente in otrosComponentes)
        {
            if (componente is MonoBehaviour monoComponente)
            {
                monoComponente.enabled = true;
            }
        }
    }
    // Función para salir al menú principal
    public void SalirAlMenu()
    {
        Time.timeScale = 1f; // Reanudar el juego
        AudioListener.pause = false; // Reanudar el audio
        SceneManager.LoadScene(0); // Cargar la primera escena (ajusta el índice si es necesario)
    }

    // Función para reiniciar el juego
    public void ReiniciarJuego()
    {
        Time.timeScale = 1f; // Reanudar el juego
        AudioListener.pause = false; // Reanudar el audio
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Recargar la escena actual
    }
    // Función que hace desaparecer al jugador
    void DesaparecerJugador(GameObject jugador)
    {
        StartCoroutine(DesaparecerJugadorConRetraso(jugador));
    }

    IEnumerator DesaparecerJugadorConRetraso(GameObject jugador)
    {
      

        if (audioDesaparicion != null)
        {
            audioDesaparicion.Play();
        }
        jugador.SetActive(false);
        yield return new WaitForSeconds(2f);
        jugador.SetActive(true);
        StartCoroutine(MostrarPanelConRetraso());
    }
    IEnumerator MostrarPanelConRetraso()
    {
        yield return new WaitForSeconds(5f); // Espera de 5 segundos
        panelFinJuego.SetActive(true); // Muestra el panel
    }
}
