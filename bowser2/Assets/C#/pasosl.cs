using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pasosl : MonoBehaviour
{
   
    public AudioSource sonidoConcreto;
    public AudioSource sonidoTierra;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Concreto"))
        {
            sonidoConcreto.Play();
        }
        else if (other.gameObject.CompareTag("Terrain"))
        {
            sonidoTierra.Play();
        }
    }
}