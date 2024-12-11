using UnityEngine;

public class AsignarJugador : MonoBehaviour
{
    public GameObject jugador; // Arrastra el jugador al Inspector

    public void AsignarJugadorAlObjeto(GameObject jugador)
    {
        this.jugador = jugador;
    }
}
