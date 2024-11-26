using UnityEngine;

public class PanelControl : MonoBehaviour
{
    public GameObject panel;          // El panel que se mostrará
    public AudioSource audioSource;   // Fuente de sonido
    public float delayBeforePanel = 1f;  // Tiempo antes de mostrar el panel (1 segundo)
    public float panelDuration = 2f;    // Duración del panel visible (2 segundos)

    private void Start()
    {
        // Iniciar la corutina para manejar la lógica del panel
        StartCoroutine(MostrarPanelConSonido());
    }

    private System.Collections.IEnumerator MostrarPanelConSonido()
    {
        yield return new WaitForSeconds(delayBeforePanel); // Esperar 1 segundo
        panel.SetActive(true); // Activar el panel
        if (audioSource != null)
        {
            audioSource.Play(); // Reproducir el sonido
        }
        yield return new WaitForSeconds(panelDuration); // Mantener el panel visible por 2 segundos
        panel.SetActive(false); // Ocultar el panel
    }
}
