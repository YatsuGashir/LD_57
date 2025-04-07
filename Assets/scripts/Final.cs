using UnityEngine;

public class Final : MonoBehaviour
{
    [SerializeField] private Canvas WinCanvas;
    private void OnTriggerEnter2D(Collider2D other)
    {
        WinCanvas.gameObject.SetActive(true);
        Time.timeScale = 0;
    }
}
