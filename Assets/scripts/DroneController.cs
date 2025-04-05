using UnityEngine;

public class DroneController : MonoBehaviour
{
    public static DroneController instance;
    
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject box;
    
    private Vector3 targetPosition;
    public bool isActive = false;
    private float clickCooldown = 0.5f; // Задержка между переключениями
    private float lastClickTime;

    void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if (Input.GetMouseButton(1) && Time.time - lastClickTime > clickCooldown)
        {
            lastClickTime = Time.time;
            
            if (isActive)
            {
                GameManager.instance.DrillingStage();
            }
            else
            {
                GameManager.instance.MiningStage();
            }
            isActive = !isActive;
        }

        if (isActive)
        {
            spriteRenderer.enabled = true;
            Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cursorPosition.z = 0f;
            targetPosition = cursorPosition;
            MoveTowardsTarget();
        }
        else
        {
            Vector3 cursorPosition = box.transform.position;
            cursorPosition.z = 0f;
            targetPosition = cursorPosition;
            MoveTowardsTarget();
                //spriteRenderer.enabled = false;
        }
    }

    private void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }
}