using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Text))]
public class HighFruitsCollected : MonoBehaviour {
    Text highFruitCollected;
    void OnEnable()
    {
        highFruitCollected = GetComponent<Text>();
        highFruitCollected.text = "Fruits: " + PlayerPrefs.GetInt("FruitCollected").ToString();
    }
}