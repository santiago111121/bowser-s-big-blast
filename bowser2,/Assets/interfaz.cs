using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class interfaz : MonoBehaviour
{
    [SerializeField] private GameObject botonPausa;
    [SerializeField] private GameObject menuPausa;

    private bool JuegoPausado = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (JuegoPausado)
            {
                Reanudar();
            }
            else
            {
                pausa();
            }
        }
    }

    public void pausa()
    {
        JuegoPausado = true;
        Time.timeScale = 0f;
        botonPausa.SetActive(false);
        menuPausa.SetActive(true);
    }

    public void Reanudar()
    {
        JuegoPausado = false;
        Time.timeScale = 1f;
        botonPausa.SetActive(true);
        menuPausa.SetActive(false);
    }

    public void Reiniciar()
    {
        JuegoPausado = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Cerrar()
    {
        Debug.Log("saliendo...");
        SceneManager.LoadScene(0);
    }
}
