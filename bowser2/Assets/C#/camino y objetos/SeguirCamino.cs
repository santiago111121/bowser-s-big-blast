using UnityEngine;
using System.Collections;

public class SeguirCamino : MonoBehaviour
{
    public Transform[] puntos; // Array de puntos para seguir el camino.
    public Transform[] caminoIncorrecto; // Nuevo camino para el objeto incorrecto.
    public float velocidad = 5f; // Velocidad de movimiento.
    public float velocidadRotacion = 5f; // Velocidad para rotar hacia la dirección del movimiento.
    public float distanciaCercania = 0.2f; // Distancia para considerar que llegó al punto objetivo.
    public float retrasoEntrePuntos = 0f; // Tiempo de espera antes de avanzar al siguiente punto (cambiado a 0 para evitar pausas).
    private int indiceActual = 0; // Índice del punto objetivo actual.
    private bool completado = false; // Indica si el recorrido ha sido completado.
    private bool rotacionFinalIniciada = false; // Indica si inició la rotación hacia el eje Z.
    private bool esperando = false; // Controla si está esperando antes de moverse.

    private bool usandoCaminoIncorrecto = false; // Indica si estamos en el camino incorrecto.

    // NUEVO: Contador para el retraso del camino incorrecto.
    public float retrasoCaminoIncorrecto = 2f;
    private bool iniciandoCaminoIncorrecto = false;

    // NUEVO: Referencia al Animator.
    public Animator animator;

    private Vector3 posicionAnterior; // Guarda la posición previa para calcular la velocidad.

    // NUEVO: Multiplicador para la velocidad de la animación.
    public float multiplicadorVelocidadAnimacion = 1f;

    void Start()
    {
        // Inicializar la posición anterior.
        posicionAnterior = transform.position;
    }

    void Update()
    {
        if (!completado && !esperando)
        {
            MoverHaciaPunto();
        }
        else if (completado && !rotacionFinalIniciada)
        {
            IniciarRotacionFinal();
        }
        else if (rotacionFinalIniciada)
        {
            RotarLentamenteAlEjeZ();
        }

        ActualizarAnimator(); // Llamar a la función que controla el Animator.

        // NUEVO: Reiniciar el camino si se presiona la tecla Space.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ActivarPrimerCamino();
        }
    }

    void MoverHaciaPunto()
    {
        Transform[] caminoActual = usandoCaminoIncorrecto ? caminoIncorrecto : puntos;

        if (indiceActual >= caminoActual.Length)
        {
            completado = true; // Marcamos el recorrido como completado.
            return;
        }

        // Si se activa el camino incorrecto, espera antes de moverse.
        if (usandoCaminoIncorrecto && !iniciandoCaminoIncorrecto)
        {
            StartCoroutine(RetardoCaminoIncorrecto());
            return;
        }

        // Obtener el punto objetivo.
        Transform objetivo = caminoActual[indiceActual];
        Vector3 direccion = (objetivo.position - transform.position).normalized;

        // Ajustar la dirección para ignorar la altura (eje Y).
        direccion.y = 0;

        // Moverse hacia el punto.
        transform.position = Vector3.MoveTowards(transform.position, objetivo.position, velocidad * Time.deltaTime);

        // Girar hacia la dirección del movimiento sin afectar el eje Y.
        if (direccion.magnitude > 0.01f)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadRotacion * Time.deltaTime);
        }

        // Si estamos suficientemente cerca del punto objetivo, pasar al siguiente directamente.
        if (Vector3.Distance(transform.position, objetivo.position) <= distanciaCercania)
        {
            if (indiceActual == caminoActual.Length - 1)
            {
                // Es el último punto, detener el movimiento.
                completado = true;
            }
            else
            {
                // Avanzar al siguiente punto sin detenerse.
                indiceActual++;
            }
        }
    }

    IEnumerator RetardoCaminoIncorrecto()
    {
        iniciandoCaminoIncorrecto = true; // Marca que el contador ha iniciado.
        esperando = true;
        yield return new WaitForSeconds(retrasoCaminoIncorrecto); // Espera el tiempo configurado.
        esperando = false;
    }

    void IniciarRotacionFinal()
    {
        // Marcar que hemos iniciado la rotación hacia el eje Z.
        rotacionFinalIniciada = true;
    }

    void RotarLentamenteAlEjeZ()
    {
        // Rotar gradualmente hacia el eje Z de Unity.
        Quaternion rotacionObjetivo = Quaternion.Euler(0, 0, 0); // Mirar hacia el eje Z positivo.
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadRotacion * Time.deltaTime);

        // Comprobar si la rotación está cerca del objetivo.
        if (Quaternion.Angle(transform.rotation, rotacionObjetivo) < 1f)
        {
            transform.rotation = rotacionObjetivo; // Fijar la rotación.
            rotacionFinalIniciada = false; // Detenemos la rotación final si es necesario.
        }
    }

    public void ActivarCaminoIncorrecto()
    {
        usandoCaminoIncorrecto = true;
        iniciandoCaminoIncorrecto = false; // Reinicia el contador.
        indiceActual = 0; // Reinicia el índice para seguir el nuevo camino.
        completado = false;
    }

    void ActualizarAnimator()
    {
        // Calcular la velocidad del objeto comparando la posición actual y la anterior.
        float velocidadActual = ((transform.position - posicionAnterior).magnitude) / Time.deltaTime;

        // Ajustar la velocidad del Animator.
        if (animator != null)
        {
            animator.SetFloat("Velocidad", velocidadActual * multiplicadorVelocidadAnimacion);
        }

        // Actualizar la posición anterior.
        posicionAnterior = transform.position;
    }

    public void ActivarPrimerCamino()
    {
        indiceActual = 0; // Reiniciar el índice.
        completado = false; // Permitir que el objeto vuelva a moverse.
        usandoCaminoIncorrecto = false; // Asegurarse de que sigue el camino correcto.
        rotacionFinalIniciada = false; // Restablecer la rotación.
        transform.position = puntos[0].position; // Colocar el objeto en el inicio del camino.
        Debug.Log("Reiniciando al primer camino.");
    }
}
