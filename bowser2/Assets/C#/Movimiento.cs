using UnityEngine;

public class Movimiento : MonoBehaviour
{
    public Transform destino;       // El destino al que se moverá el objeto principal
    public Transform hijo;          // El objeto hijo que debe quedarse estático en el destino
    private Vector3 posicionInicial;        // Guarda la posición inicial del objeto principal
    private Vector3 posicionInicialHijo;    // Guarda la posición inicial relativa del hijo
    private bool moviendoHaciaDestino = true;
    private bool regresando = false;

    public float velocidad = 3f;            // Velocidad de movimiento
    public float tiempoDeEspera = 1f;       // Tiempo de espera al llegar al destino
    public float tiempoParaReiniciar = 3f;  // Tiempo antes de reiniciar el movimiento

    private float tiempoTranscurrido = 0f;

    void Start()
    {
        // Establecer la posición inicial del objeto principal
        posicionInicial = transform.position;

        // Almacenar la posición inicial relativa del hijo al objeto principal
        if (hijo != null)
        {
            posicionInicialHijo = hijo.localPosition;
        }
    }

    void Update()
    {
        if (moviendoHaciaDestino)
        {
            // Mueve el objeto principal hacia el destino
            transform.position = Vector3.MoveTowards(transform.position, destino.position, velocidad * Time.deltaTime);

            if (transform.position == destino.position)
            {
                tiempoTranscurrido += Time.deltaTime;
                if (tiempoTranscurrido >= tiempoDeEspera)
                {
                    moviendoHaciaDestino = false;
                    regresando = true;

                    // Deja al hijo en el destino y lo libera (el hijo se queda en el destino)
                    if (hijo != null)
                    {
                        hijo.SetParent(null);
                    }
                }
            }
        }
        else if (regresando)
        {
            // Regresa el objeto principal a su posición inicial
            transform.position = Vector3.MoveTowards(transform.position, posicionInicial, velocidad * Time.deltaTime);

            if (transform.position == posicionInicial)
            {
                // El objeto principal se queda estático en su posición inicial
                regresando = false;
            }
        }
    }

    public void ReiniciarMovimiento()
    {
        // Llama a un método retrasado que reinicia todo después del tiempo configurado
        Invoke(nameof(RealizarReinicio), tiempoParaReiniciar);
    }

    private void RealizarReinicio()
    {
        // Restablece las variables para repetir el movimiento desde el principio
        transform.position = posicionInicial; // Asegura que el objeto principal vuelve a su inicio
        moviendoHaciaDestino = true;
        regresando = false;
        tiempoTranscurrido = 0f;

        // Reasigna el hijo al objeto principal y restablece su posición relativa
        if (hijo != null)
        {
            hijo.SetParent(transform);
            hijo.localPosition = posicionInicialHijo; // Restaura la posición relativa inicial del hijo
        }
    }
}
