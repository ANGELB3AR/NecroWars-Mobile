using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] Slider progressBarSlider;

    private Progression progression;

    private void Awake()
    {
        progression = FindObjectOfType<Progression>();
    }

    private void OnEnable()
    {
        progression.OnHoardDefeated += Progression_OnHoardDefeated;
    }

    private void OnDisable()
    {
        progression.OnHoardDefeated -= Progression_OnHoardDefeated;
    }

    private void Start()
    {
        //progressBarSlider.maxValue = progression.GetStartingHoardQuantity();
        progressBarSlider.value = 0f;
    }

    private void Update()
    {
        if (progressBarSlider.maxValue > 1) { return; }
        else
        {
            //progressBarSlider.maxValue = progression.GetStartingHoardQuantity();
        }
    }

    private void Progression_OnHoardDefeated()
    {
        progressBarSlider.value++;
    }
}
