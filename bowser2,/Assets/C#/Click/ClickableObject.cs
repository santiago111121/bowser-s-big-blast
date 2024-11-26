using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    public MoveToObject mover; // Referencia al script del personaje

    private void OnMouseDown()
    {
        if (mover != null)
        {
            mover.MoveTo(transform.position); // Mueve al personaje a la posición de este objeto
        }
    }
}
