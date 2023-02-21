using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    Slider slide;
    float health = 5;

    // Start is called before the first frame update
    void Start()
    {
        slide = GetComponent<Slider>();

    }

    // Update is called once per frame
    void Update()
    {
        health -= 1 * Time.deltaTime;
        slide.value = health;
    }
}
