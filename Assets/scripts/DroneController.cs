using UnityEngine;

public class DroneController : MonoBehaviour
{
 [SerializeField] private float moveSpeed = 5f; // Скорость движения дрона
    [SerializeField] private float rotationSpeed = 100f; // Скорость вращения дрона
    [SerializeField] private float orbitRadius = 2f; // Радиус орбиты вокруг точки
    [SerializeField] private float orbitSpeed = 50f; // Скорость орбитального движения
    [SerializeField] private float orbitZoneRadius = 1f; // Радиус зоны для включения орбитального режима

    private Vector3 targetPosition;
    private bool isOrbiting = false;

    private void Update()
    {
        // Получаем позицию курсора на экране и конвертируем её в мировые координаты
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorPosition.z = 0f;  // Убираем смещение по оси Z, чтобы работать в 2D пространстве
        targetPosition = cursorPosition;
        MoveTowardsTarget();
        // Если дрон не орбитирует, он должен двигаться к точке
        /*if (!isOrbiting)
        {
            targetPosition = cursorPosition;
            MoveTowardsTarget();
        }
        else
        {
            // Если дрон орбитирует, начинаем орбитальное движение вокруг цели
            OrbitAroundTarget(cursorPosition);
        }*/
    }

    private void MoveTowardsTarget()
    {
        // Двигаем дрон в сторону курсора
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Проверяем, попал ли дрон в радиус орбитальной зоны
        if (Vector3.Distance(transform.position, targetPosition) < orbitZoneRadius)
        {
            isOrbiting = true;  // Начинаем орбитальное движение
        }
    }

   /*private void OrbitAroundTarget(Vector3 cursorPosition)
    {
        // Крутим дрон вокруг целевой точки (позиции курсора)
        float angle = orbitSpeed * Time.deltaTime;
        Vector3 offset = new Vector3(Mathf.Sin(Time.time * angle) * orbitRadius, Mathf.Cos(Time.time * angle) * orbitRadius, 0);
        transform.position = cursorPosition + offset;
        

        // Если дрон выходит из радиуса орбитальной зоны, снова начинаем следовать за курсором
        if (Vector3.Distance(transform.position, cursorPosition) > orbitZoneRadius)
        {
            isOrbiting = false;
        }
    }*/
}
