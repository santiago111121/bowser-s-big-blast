using UnityEngine;

public class MoverEntreElementos : MonoBehaviour
{
    public Transform[] elementos;        // Array con los 5 elementos de referencia
    public float velocidad = 5f;         // Velocidad de movimiento
    private int indiceActual = 0;        // Índice del elemento al que se mueve el personaje
    private bool enMovimiento = false;  // Indica si el personaje está en movimiento
    public Animator animator;           // Referencia al componente Animator

    void Update()
    {
        // Solo mover si no está ya en movimiento
        if (!enMovimiento)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) && indiceActual < elementos.Length - 1)
            {
                // Mirar hacia la derecha (X positivo)
                GirarHaciaDireccion(Vector3.right);
                indiceActual++;
                StartCoroutine(MoverHaciaElemento(elementos[indiceActual].position));
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) && indiceActual > 0)
            {
                // Mirar hacia la izquierda (X negativo)
                GirarHaciaDireccion(Vector3.left);
                indiceActual--;
                StartCoroutine(MoverHaciaElemento(elementos[indiceActual].position));
            }
        }

        // Control de animaciones con las teclas Z y X
        if (Input.GetKeyDown(KeyCode.Z))
        {
            animator.SetBool("Other", false); // Desactivar otras animaciones si es necesario
            animator.Play("Boton");          // Reproducir la animación "Boton"
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            animator.SetBool("Other", false); // Desactivar otras animaciones si es necesario
            animator.Play("Baila");          // Reproducir la animación "Baila"
        }
        else if (!Input.anyKey) // Cuando no se presiona ninguna tecla
        {
            animator.SetBool("Other", true); // Activar animación "Other" como estado predeterminado
        }
    }

    private System.Collections.IEnumerator MoverHaciaElemento(Vector3 destino)
    {
        enMovimiento = true; // Marcar que el personaje está en movimiento

        // Mantener la posición Y constante
        destino.y = transform.position.y;

        // Moverse hacia el destino de forma suave
        while (Vector3.Distance(transform.position, destino) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                destino,
                velocidad * Time.deltaTime
            );
            yield return null; // Esperar al siguiente frame
        }

        // Asegurarse de que el personaje termine exactamente en el destino
        transform.position = destino;

        // Girar hacia el eje Z positivo al detenerse
        GirarHaciaDireccion(Vector3.forward);

        enMovimiento = false; // Marcar que el movimiento ha terminado
    }

    private void GirarHaciaDireccion(Vector3 direccion)
    {
        // Ajustar la rotación para mirar en la dirección especificada
        Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
        transform.rotation = rotacionObjetivo;
    }
}
