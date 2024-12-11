using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class interfaz : MonoBehaviour
{
    [SerializeField] private GameObject botonPausa;
    [SerializeField] private GameObject menuPausa;
    [SerializeField] private GameObject nuevoPanel;  // Nuevo panel añadido
    [SerializeField] private Button botonReiniciar;  // Botón de reiniciar
    [SerializeField] private Button botonSalir;  // Botón de salir

    private bool JuegoPausado = false;

    // Sonidos
    public AudioClip sonidoPausa;  // Sonido al pausar
    public AudioClip sonidoHover;  // Sonido al pasar el cursor sobre los botones
    public AudioClip sonidoBotonClick;  // Sonido al hacer clic en los botones
    private AudioSource audioSource;

    // Controlador del volumen del sonido de hover
    [Range(0f, 1f)] public float volumenHover = 0.3f;  // Control de volumen (rango entre 0 y 1)

    private void Start()
    {
        // Añadir el componente AudioSource si no existe
        audioSource = gameObject.AddComponent<AudioSource>();

        // Asignar sonido de hover a los botones dentro del menú de pausa
        Button[] botones = menuPausa.GetComponentsInChildren<Button>();
        foreach (Button boton in botones)
        {
            // Crear y agregar el EventTrigger si no existe
            EventTrigger trigger = boton.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = boton.gameObject.AddComponent<EventTrigger>();
            }

            // Configurar evento OnPointerEnter para reproducir el sonido al pasar el cursor
            EventTrigger.Entry hoverEntry = new EventTrigger.Entry();
            hoverEntry.eventID = EventTriggerType.PointerEnter;
            hoverEntry.callback.AddListener((eventData) => ReproducirSonido(sonidoHover, volumenHover));
            trigger.triggers.Add(hoverEntry);

            // Configurar evento OnClick para reproducir el sonido al hacer clic
            boton.onClick.AddListener(() => ReproducirSonido(sonidoBotonClick, 1f));  // Sonido de click con volumen normal
        }

        // Asignar evento de clic al botón de pausa
        Button botonPausaComponent = botonPausa.GetComponent<Button>();
        if (botonPausaComponent != null)
        {
            botonPausaComponent.onClick.AddListener(Pausa);  // Pausar juego cuando se haga clic en el botón
        }

        // Asignar eventos a los botones del nuevo panel
        if (botonReiniciar != null)
        {
            botonReiniciar.onClick.AddListener(Reiniciar);  // Reiniciar juego al hacer clic
        }

        if (botonSalir != null)
        {
            botonSalir.onClick.AddListener(Salir);  // Salir y cargar la primera escena al hacer clic
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (JuegoPausado)
            {
                Reanudar();
            }
            else
            {
                Pausa();
            }
        }
    }

    public void Pausa()
    {
        JuegoPausado = true;
        Time.timeScale = 0f;
        botonPausa.SetActive(false);
        menuPausa.SetActive(true);
        nuevoPanel.SetActive(false);  // Asegurarse de que el nuevo panel está desactivado

        // Reproducir sonido al pausar el juego
        ReproducirSonido(sonidoPausa, 1f);  // Sonido de pausa con volumen normal
    }

    public void Reanudar()
    {
        JuegoPausado = false;
        Time.timeScale = 1f;
        botonPausa.SetActive(true);
        menuPausa.SetActive(false);
    }

    public void Reiniciar()
    {
        JuegoPausado = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);  // Reinicia la escena actual
    }

    public void Salir()
    {
        Debug.Log("Regresando a la primera escena...");
        SceneManager.LoadScene(0);  // Cargar la primera escena
    }

    private void ReproducirSonido(AudioClip clip, float volumen)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip, volumen);
        }
    }

    // Nuevo método para mostrar el nuevo panel
    public void MostrarNuevoPanel()
    {
        nuevoPanel.SetActive(true);
        menuPausa.SetActive(false);  // Ocultar el menú de pausa cuando se muestra el nuevo panel
    }
}
