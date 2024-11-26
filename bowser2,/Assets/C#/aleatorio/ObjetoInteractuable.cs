using UnityEngine;

public class ObjetoInteractuable : MonoBehaviour
{
    public InteraccionAleatoria controlador;
    public int indiceObjeto;

    private bool jugadorEnRango = false;

    void Update()
    {
        if (jugadorEnRango && Input.GetKeyDown(KeyCode.Z))
        {
            controlador.InteractuarConObjeto(indiceObjeto);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnRango = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnRango = false;
        }
    }
}
