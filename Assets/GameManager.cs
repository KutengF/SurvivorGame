using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public SpawnManager spawnManager;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI timeText;
    public GameObject gameOverPanel;

    private float elapsedTime = 0f;
    private bool gameIsOver = false;

    void Start()
    {
        StartCoroutine(UpdateTimeAndWave());
    }

    void Update()
    {
        if (!gameIsOver)
        {
            elapsedTime += Time.deltaTime;
            UpdateUI();
        }
    }

    IEnumerator UpdateTimeAndWave()
    {
        while (!spawnManager.IsGameOver())
        {
            yield return null;
        }

        EndGame();
    }

    void UpdateUI()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(elapsedTime - minutes * 60);
        string timeString = string.Format("{0:0}:{1:00}", minutes, seconds);

        waveText.text = "Wave: " + (spawnManager.GetCurrentWaveIndex() + 1);
        timeText.text = "Time: " + timeString;
    }

    void EndGame()
    {
        gameIsOver = true;
        Time.timeScale = 0; // Pause the game
        gameOverPanel.SetActive(true);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1; // Unpause the game
        SceneManager.LoadScene("MainMenu");
    }
}
