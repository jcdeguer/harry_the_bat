using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;

    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject countdownPage;
    public GameObject cherry;
    public GameObject stalactites;
    public GameObject cave;
    public GameObject ground;
    public GameObject bat;

    public Text scoreText;
    public Text fruitText;
    int score; // inicializa la variable contador con 0
    int fruit; // inicializa la variable contador con 0
    int highScoreCounter;
    public AudioSource highScoreAudio;
    bool gameOver = true; // Al iniciar el juego, el estado gameOver esta desactivado
    //public AudioSource gameOverAudio;
    int scoreSpeederCounter = 2;

    enum PageState
    {
        None,
        Start,
        GameOver,
        Countdown
    }

    public bool GameOver
    {
        get
        {
            return gameOver;
        }
    }

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    { // Metodo que habilita metodos externos, considerar siempre que se deben crear los metodos
        CountdownText.OnCountdownFinished += OnCountdownFinished;
        BatController.OnPlayerDied += OnPlayerDied;
        BatController.OnPlayerScore += OnPlayerScore;
        BatController.OnPlayerCollectFruit += OnPlayerCollectFruit;
    }

    private void OnDisable()
    {
        CountdownText.OnCountdownFinished -= OnCountdownFinished;
        BatController.OnPlayerDied -= OnPlayerDied;
        BatController.OnPlayerScore -= OnPlayerScore;
        BatController.OnPlayerCollectFruit -= OnPlayerCollectFruit;
    }

    void OnCountdownFinished()
    { // Metodo que se ejecuta al 
        SetPageState(PageState.None);
        OnGameStarted(); // Evento que inicia el juego, se envia al BatController
        scoreText.text = "0";
        fruitText.text = "0";
        cherry.GetComponent<Parallaxer>().shiftSpeed = (float)1.25;
        stalactites.GetComponent<Parallaxer>().shiftSpeed = (float)1.25;
        cave.GetComponent<Parallaxer>().shiftSpeed = (float)1.25;
        ground.GetComponent<Parallaxer>().shiftSpeed = (float)1.25;
        cherry.GetComponent<Parallaxer>().spawnRate = (float)3.5;
        stalactites.GetComponent<Parallaxer>().spawnRate = (float)3.5;
        cave.GetComponent<Parallaxer>().spawnRate = (float)0;
        ground.GetComponent<Parallaxer>().spawnRate = (float)0;
        bat.GetComponent<BatController>().tabForce = 200;
        score = 0;
        highScoreCounter = 10;
        fruit = 0;
        gameOver = false;
        scoreSpeederCounter = 2;
    }

    void OnPlayerScore()
    {
        score++;
        scoreText.text = score.ToString();
        OnGameSpeeder();
        if (score%2 == 0) // Chequea que cada dos cruces, se consume una fruta
        {
            if (fruit <= 0)
            {
                bat.GetComponent<BatController>().tabForce = bat.GetComponent<BatController>().tabForce - 20;
            }
            if (fruit > 0)
            {
                fruit--; // se consume una fruta
                fruitText.text = fruit.ToString();
            }
        }
        if (score == highScoreCounter)
        {
            highScoreCounter = highScoreCounter + 10;
            highScoreAudio.Play();
        }

    }

    void OnGameSpeeder()
    {
        if (score >= scoreSpeederCounter)
        {
            scoreSpeederCounter = score;
            scoreSpeederCounter = scoreSpeederCounter + 1;

            if (cherry.GetComponent<Parallaxer>().shiftSpeed <= 3)
            {
                cherry.GetComponent<Parallaxer>().shiftSpeed = (float)(cherry.GetComponent<Parallaxer>().shiftSpeed + 0.08);
                stalactites.GetComponent<Parallaxer>().shiftSpeed = (float)(stalactites.GetComponent<Parallaxer>().shiftSpeed + 0.08);
                cave.GetComponent<Parallaxer>().shiftSpeed = (float)(cave.GetComponent<Parallaxer>().shiftSpeed + 0.08);
                ground.GetComponent<Parallaxer>().shiftSpeed = (float)(ground.GetComponent<Parallaxer>().shiftSpeed + 0.08);
            }
            if (cherry.GetComponent<Parallaxer>().spawnRate >= 1.5)
            {
                cherry.GetComponent<Parallaxer>().spawnRate = (float)(cherry.GetComponent<Parallaxer>().spawnRate - 0.1);
                stalactites.GetComponent<Parallaxer>().spawnRate = (float)(stalactites.GetComponent<Parallaxer>().spawnRate - 0.1);
            }
        }
    }

    void OnPlayerCollectFruit()
    {
        fruit++;
        fruitText.text = fruit.ToString();
        bat.GetComponent<BatController>().tabForce = bat.GetComponent<BatController>().tabForce + 20;
        if (bat.GetComponent<BatController>().tabForce >= 200)
        {
            bat.GetComponent<BatController>().tabForce = 200;
        }
    }

    void OnPlayerDied()
    {
        gameOver = true;
        int savedScore = PlayerPrefs.GetInt("HighScore");
        if (score > savedScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
        SetPageState(PageState.GameOver);
        //gameOverAudio.Play();
    }

    void SetPageState(PageState state)
    {
        switch (state)
        {
            case PageState.None:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.Countdown:
                startPage.SetActive(false);
                countdownPage.SetActive(true);
                gameOverPage.SetActive(false);
                break;
            case PageState.GameOver:
                startPage.SetActive(false);
                countdownPage.SetActive(false);
                gameOverPage.SetActive(true);
                break;
        }
    }

    public void ConfirmGameOver()
    {
        // Se activa cuando se presiona el boton de recargar
        OnGameOverConfirmed(); //Evento que relanza el juego, se envia al BatController
        SetPageState(PageState.Start);
    }

    public void StartGame()
    {
        // Se activa cuando se presiona el boton de iniciar
        SetPageState(PageState.Countdown);
    }
}
