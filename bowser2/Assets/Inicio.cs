using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inicio : MonoBehaviour
{
    public AudioClip sonidoHover; // Sonido al pasar el cursor
    public AudioClip sonidoClick; // Sonido al hacer clic
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        // Asignar sonido de hover a todos los botones hijos
        Button[] botones = GetComponentsInChildren<Button>();
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
            hoverEntry.callback.AddListener((eventData) => ReproducirSonido(sonidoHover));
            trigger.triggers.Add(hoverEntry);

            // Configurar evento OnClick para reproducir el sonido al hacer clic
            boton.onClick.AddListener(() => ReproducirSonido(sonidoClick));
        }
    }

    public void Jugar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Salir()
    {
        Debug.Log("Salir...");
        Application.Quit();
    }

    private void ReproducirSonido(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
