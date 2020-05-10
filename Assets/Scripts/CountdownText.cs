using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]

public class CountdownText : MonoBehaviour
{
    public delegate void CountdownFinished();
    public static event CountdownFinished OnCountdownFinished;

    Text countdown;

    void OnEnable() // Metodo que muestra el contador al iniciar el juego
    {
        countdown = GetComponent<Text>();
        countdown.text = "3";
        StartCoroutine("Countdown");
    }

    IEnumerator Countdown() // Metodo que realiza la cuenta regresiva de 3 a 1 al iniciar el juego
    {
        int count = 3;
        for (int i = 0; i < count; i++)
        {
            countdown.text = (count - i).ToString();
            yield return new WaitForSeconds(1);
        }

        OnCountdownFinished(); //Evento que se envia al GameManager
    }

}
