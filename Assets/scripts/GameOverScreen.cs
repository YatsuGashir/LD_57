using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    [Header("Настройки экрана")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private float showDelay = 1f; // Задержка перед показом экрана

    private void Awake()
    {
        // Скрываем панель при старте
        gameOverPanel.SetActive(false);
        
        // Назначаем обработчики кнопок
        restartButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(QuitGame);
        
        // Подписываемся на событие уничтожения платформы
        PlatformHealth.OnPlatformDestroyed += ShowGameOverScreen;
    }

    private void OnDestroy()
    {
        // Отписываемся от события при уничтожении объекта
        PlatformHealth.OnPlatformDestroyed -= ShowGameOverScreen;
    }

    private void ShowGameOverScreen()
    {
        StartCoroutine(ShowScreenWithDelay());
    }

    private System.Collections.IEnumerator ShowScreenWithDelay()
    {
        yield return new WaitForSeconds(showDelay);
        
        // Активируем панель
        gameOverPanel.SetActive(true);
        
        // Пауза игры
        Time.timeScale = 0f;
    }

    private void RestartGame()
    {
        // Восстанавливаем нормальную скорость игры
        Time.timeScale = 1f;
        
        // Перезагружаем текущую сцену
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void QuitGame()
    {
        // Выход из игры (в редакторе остановит проигрывание)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}