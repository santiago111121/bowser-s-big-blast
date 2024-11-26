using System.Collections;
using UnityEngine;

public class BajaHijo : MonoBehaviour
{
    public GameObject hijo; // Referencia al hijo del objeto
    private bool dentroDelCollider = false; // Para saber si el jugador está dentro del collider
    private bool enProceso = false; // Evitar múltiples activaciones simultáneas
    private Vector3 posicionInicialHijo; // Almacena la posición inicial del hijo

    private void Start()
    {
        // Guarda la posición inicial del hijo
        if (hijo != null)
        {
            posicionInicialHijo = hijo.transform.localPosition;
        }
    }

    private void Update()
    {
        // Verificar si el jugador presiona la tecla 'Z' y está dentro del collider
        if (dentroDelCollider && Input.GetKeyDown(KeyCode.Z) && !enProceso)
        {
            // Inicia la corutina para bajar el hijo
            StartCoroutine(BajarHijoLentamente());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto que entra al collider tiene el tag "Player"
        if (other.CompareTag("Player"))
        {
            dentroDelCollider = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Salir del collider y verificar si el objeto es el "Player"
        if (other.CompareTag("Player"))
        {
            dentroDelCollider = false;
        }
    }

    private IEnumerator BajarHijoLentamente()
    {
        enProceso = true; // Bloquear nuevas activaciones
        yield return new WaitForSeconds(2); // Esperar 2 segundos

        // Mover al hijo hacia abajo lentamente
        if (hijo != null)
        {
            Vector3 posicionInicial = hijo.transform.localPosition;
            Vector3 posicionFinal = new Vector3(
                posicionInicial.x,
                posicionInicial.y / 2, // Baja la mitad de su tamaño
                posicionInicial.z
            );

            float duracion = 1f; // Duración del movimiento (en segundos)
            float tiempo = 0;

            while (tiempo < duracion)
            {
                hijo.transform.localPosition = Vector3.Lerp(posicionInicial, posicionFinal, tiempo / duracion);
                tiempo += Time.deltaTime;
                yield return null;
            }

            // Asegurar la posición final
            hijo.transform.localPosition = posicionFinal;
        }

        enProceso = false; // Permitir nuevas activaciones
    }

    public void SubirHijo()
    {
        // Subir la posición del hijo de vuelta a su estado inicial
        if (hijo != null)
        {
            hijo.transform.localPosition = posicionInicialHijo;
        }

        Debug.Log("Posición del hijo reiniciada.");
    }

    public void ReiniciarPosicionHijos()
    {
        // Subir al hijo a su posición original cuando se reinicia
        SubirHijo();
    }

    // Método para subir el hijo cuando no sea el objeto correcto
    public void SubirHijoSiEsIncorrecto(bool esCorrecto)
    {
        if (!esCorrecto)
        {
            SubirHijo();
        }
    }
}
