using UnityEngine;
using UnityEngine.Events;

public class MeshButtonHandler : MonoBehaviour
{
    public UnityEvent OnClicked;
    public void OnMouseUpAsButton()
    {
        OnClicked.Invoke();
    }
}
