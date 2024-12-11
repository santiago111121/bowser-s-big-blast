using UnityEngine;

public class Movimiento : MonoBehaviour
{
    public Transform destino;            // El destino al que se moverá el objeto principal
    public Transform hijo;               // El objeto hijo que debe quedarse estático en el destino
    private Vector3 posicionInicial;     // Guarda la posición inicial del objeto principal
    private Vector3 posicionInicialHijo; // Guarda la posición inicial relativa del hijo
    private bool moviendoHaciaDestino = true;
    private bool regresando = false;

    public float velocidad = 3f;         // Velocidad de movimiento
    public float tiempoDeEspera = 1f;    // Tiempo de espera al llegar al destino
    public float tiempoParaReiniciar = 3f; // Tiempo antes de reiniciar el movimiento

    private float tiempoTranscurrido = 0f;

    public AudioSource sonidoMovimiento; // Sonido para movimiento y retorno
    public AudioSource sonidoReposo;     // Sonido cuando llega al destino

    private bool sonidoDestinoReproducido = false; // Asegura que el sonido solo se reproduce una vez

    void Start()
    {
        posicionInicial = transform.position;

        if (hijo != null)
        {
            posicionInicialHijo = hijo.localPosition;
        }
    }

    void Update()
    {
        if (moviendoHaciaDestino)
        {
            if (!sonidoMovimiento.isPlaying)
            {
                sonidoMovimiento.Play();
            }

            transform.position = Vector3.MoveTowards(transform.position, destino.position, velocidad * Time.deltaTime);

            if (transform.position == destino.position)
            {
                if (!sonidoDestinoReproducido) // Verifica si el sonido ya fue reproducido
                {
                    sonidoReposo.Play();  // Reproduce el sonido de reposo al llegar al destino
                    sonidoDestinoReproducido = true;
                }

                sonidoMovimiento.Stop();

                tiempoTranscurrido += Time.deltaTime;
                if (tiempoTranscurrido >= tiempoDeEspera)
                {
                    moviendoHaciaDestino = false;
                    regresando = true;

                    if (hijo != null)
                    {
                        hijo.SetParent(null);
                    }
                }
            }
        }
        else if (regresando)
        {
            if (!sonidoMovimiento.isPlaying)
            {
                sonidoMovimiento.Play();
                sonidoReposo.Stop();  // Detiene el sonido de reposo cuando regresa
            }

            transform.position = Vector3.MoveTowards(transform.position, posicionInicial, velocidad * Time.deltaTime);

            if (transform.position == posicionInicial)
            {
                regresando = false;
                sonidoMovimiento.Stop();
                sonidoDestinoReproducido = false; // Reinicia el estado del sonido para el siguiente ciclo
            }
        }
    }

    public void ReiniciarMovimiento()
    {
        Invoke(nameof(RealizarReinicio), tiempoParaReiniciar);
    }

    private void RealizarReinicio()
    {
        transform.position = posicionInicial;
        moviendoHaciaDestino = true;
        regresando = false;
        tiempoTranscurrido = 0f;

        if (hijo != null)
        {
            hijo.SetParent(transform);
            hijo.localPosition = posicionInicialHijo;
        }

        sonidoDestinoReproducido = false; // Reinicia el estado del sonido para el siguiente movimiento
    }
}
