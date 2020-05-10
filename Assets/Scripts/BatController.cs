using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] // Si se arrastra BatController sobre un objeto crea un rigidbody automaticamente
[RequireComponent(typeof(int))]

public class BatController : MonoBehaviour{

    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScore;
    public static event PlayerDelegate OnPlayerCollectFruit;

    public float tabForce = 220; // Recuperara o ganar altura al presionar
    public float tiltSmooth = 5; // Hara que rote al caer
    public Vector3 startPosition; // Posicion inicial al comenzar el juego
    new Rigidbody2D rigidbody;
    Quaternion downRotation; // Quaternion se encarga de rota en cuatro direcciones un objeto, sentido horario y antihorario
    Quaternion fordwardRotation; //
    GameManager game;
    public Sprite fallingSprite;
    public Sprite startingSprite;

    public AudioSource flapAudio;
    public AudioSource scoreAudio;
    public AudioSource dieAudio;
    public AudioSource fruitAudio;

    void Start()
    { // Aqui setea las condiciones al iniciar
        rigidbody = GetComponent<Rigidbody2D>(); // Inicializa el objeto
        downRotation = Quaternion.Euler(0, 0, -90); // Indica que debe empezar a rotar en sentido horario hasta alcanzar los 90 grados
        fordwardRotation = Quaternion.Euler(0, 0, 40); // Cuando es llamado hara que rote en sentido antihorario
        game = GameManager.Instance;
        startPosition = new Vector3(-(3/2), 1, 0);
        rigidbody.simulated = false;
        GetComponent<Animator>().enabled = false;
    }

    void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameStarted()
    {
        rigidbody.velocity = Vector3.zero; // Resetea la velocidad de accion asi no es acumulativa
        rigidbody.simulated = true; // Activa caida libre
        this.GetComponent<SpriteRenderer>().sprite = startingSprite;
    }

    void OnGameOverConfirmed()
    { // Metodo que se encarga de poner todo al inicio desde cero
        transform.localPosition = startPosition; // Reseta la posicion a la inicial
        transform.rotation = Quaternion.identity; // resetea la rotasion a la posicion inicial
        this.GetComponent<SpriteRenderer>().sprite = startingSprite;
    }

    void Update()
    { // Modulo que actualiza constantemente
        if (game.GameOver) return;
        if (Input.GetMouseButtonDown(0))
        { // Accion ejecutada al presionar click derecho del mouse, o tocar la pantalla del android
            flapAudio.Play();
            transform.rotation = fordwardRotation; // Aca le decimos que rote al ejecutar fordwardRotation al recibir click
            rigidbody.velocity = Vector3.zero; // Resetea la velocidad de accion asi no es acumulativa
            rigidbody.AddForce(Vector2.up * tabForce, ForceMode2D.Force);
            GetComponent<Animator>().enabled = true;
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
        if (Quaternion.Angle(transform.rotation, downRotation) <= 60f)  // Funcion que detiene la animacion cuando la caida supera los 60 grados de inclinacion
        {
            this.GetComponent<SpriteRenderer>().sprite = fallingSprite;
            GetComponent<Animator>().enabled = false;
        }
        /*
         * Lerp se usa para indicar una rotacion desde una posicion a otra
         * Aqui se usa como base transform.rotation que es el proceso que se esta ejecutando, la funcion downRotation,
         * Y por ultimo la sutileza con la que lo hara, que sera un producto del tiempo y el valor tiltSmooth
         */
    }

    void OnTriggerEnter2D(Collider2D collision)
    { // Aqui se definen las acciones que se ejecutaran al tocar una collision
        if (collision.gameObject.tag == "scoreZone")
        {
            OnPlayerScore(); // Evento enviado al GameManager
            scoreAudio.Play();
            // ejecutar un sonido
        }
        if (collision.gameObject.tag == "fruitZone")
        {
            OnPlayerCollectFruit(); // Evento enviado al GameManager
            collision.transform.position = Vector3.one * 10;
            fruitAudio.Play();

            // ejecutar un sonido
        }
        if (collision.gameObject.tag == "deadZone")
        {
            rigidbody.simulated = false; // Cuando el objeto en caida libre toca este punto se desactiva la caida libre
            GetComponent<Animator>().enabled = false;
            // Registrar fin de juego
            OnPlayerDied(); // Evento enviado al GameManager
            dieAudio.Play();
        }
    }
}
