using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    public GameObject objectToDisappear; // El objeto que desaparecerá
    public ParticleSystem particlesToActivate; // El sistema de partículas que se activará

    void Start()
    {
        if (particlesToActivate != null)
        {
            particlesToActivate.Stop(); // Asegurarse de que el sistema de partículas esté detenido al inicio
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Objeto colisionado: " + other.name);

        // Comprueba si el objeto que colisiona es el objeto objetivo
        if (other.CompareTag("Player")) // Puedes cambiar "Player" por cualquier etiqueta que uses
        {
            Debug.Log("El objeto colisionado tiene la etiqueta Player.");

            if (objectToDisappear != null)
            {
                objectToDisappear.SetActive(false); // Desactiva el objeto
                Debug.Log("Objeto desactivado: " + objectToDisappear.name);
            }

            if (particlesToActivate != null)
            {
                particlesToActivate.Play(); // Activa el sistema de partículas
                Debug.Log("Sistema de partículas activado: " + particlesToActivate.name);
            }
        }
    }
}
