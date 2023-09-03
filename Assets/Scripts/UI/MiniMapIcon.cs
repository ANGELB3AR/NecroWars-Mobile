using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapIcon : MonoBehaviour
{
    [SerializeField] private Material opposingMaterial;
    [SerializeField] private Material resurrectedMaterial;

    private Health health;
    private Renderer minimapRenderer;

    private void OnEnable()
    {
        minimapRenderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        health = GetComponentInParent<Health>();

        health.OnCreatureDied += Health_OnCreatureDied;
        health.OnCreatureResurrected += Health_OnCreatureResurrected;

        SetMaterial();
    }

    private void Health_OnCreatureResurrected()
    {
        gameObject.SetActive(true);
        SetMaterial();
    }

    private void Health_OnCreatureDied(Creature obj)
    {
        gameObject.SetActive(false);
    }

    private void SetMaterial()
    {
        minimapRenderer.material = (health.IsResurrected()) ? resurrectedMaterial : opposingMaterial;
    }
}
