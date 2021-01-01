using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private int totalEnemies = 1;

    public Text gameText, scoreText, enemiesRemainingText;

    public AudioClip gameOverSound;
    public AudioClip gameWonSound;

    public string nextLevel;

    [SerializeField]
    private Image reticle;

    [SerializeField]
    private float xMin = -25;
    [SerializeField]
    private float xMax = 25;
    [SerializeField]
    private float yMin = 4;
    [SerializeField]
    private float yMax = 25;
    [SerializeField]
    private float zMin = -25;
    [SerializeField]
    private float zMax = 25;

    public static bool isGameOver = false;

    private float score = 0f;
    private int enemiesRemaining;

    // Start is called before the first frame update
    void Start()
    {
        enemiesRemaining = totalEnemies;
        enemiesRemainingText.text = enemiesRemaining.ToString();
        isGameOver = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void IncreaseScore()
    {
        score++;
        scoreText.text = score.ToString();
        enemiesRemaining--;
        enemiesRemainingText.text = enemiesRemaining.ToString();
        if (enemiesRemaining == 0)
            LevelBeat();

    }

    public int GetEnemiesRemaining()
    {
        return enemiesRemaining;
    }

    public void LevelLost()
    {
        isGameOver = true;
        gameText.text = "Level Failed!";
        gameText.gameObject.SetActive(true);
        reticle.gameObject.SetActive(false);

        AudioSource.PlayClipAtPoint(gameOverSound, Camera.main.transform.position);
        Camera.main.GetComponent<AudioSource>().volume = 0.2f;

        Invoke("LoadCurrentLevel", 2);
    }

    public void LevelBeat()
    {
        isGameOver = true;
        gameText.gameObject.SetActive(true);
        reticle.gameObject.SetActive(false);

        AudioSource.PlayClipAtPoint(gameWonSound, Camera.main.transform.position);

        if (!string.IsNullOrEmpty(nextLevel))
        {
            gameText.text = "Level Passed!";
            Invoke("LoadNextLevel", 2);
        }
        else
        {
            gameText.text = "You Beat The Game!";
        }

    }

    void LoadNextLevel()
    {
        SceneManager.LoadScene(nextLevel);
    }

    void LoadCurrentLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public float GetXMin()
    {
        return xMin;
    }

    public float GetXMax()
    {
        return xMax;
    }

    public float GetYMin()
    {
        return yMin;
    }

    public float GetYMax()
    {
        return yMax;
    }

    public float GetZMin()
    {
        return zMin;
    }

    public float GetZMax()
    {
        return zMax;
    }
}
