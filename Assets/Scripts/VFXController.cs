using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXController : MonoBehaviour
{
    public VisualEffect hyperjump;

    private float cooldown;
    private float elapsed;

    private void Awake()
    {
        elapsed = 0f;
        cooldown = hyperjump.GetFloat("BeamLifetime") + 1.0f;
    }

    void Start()
    {
        ResetVFX();
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            hyperjump.pause = false;
            elapsed = 0f;
        }

        if (elapsed >= cooldown)
        {
            ResetVFX();
            elapsed = 0f;
        }
    }

    private void ResetVFX()
    {
        hyperjump.gameObject.SetActive(false);
        hyperjump.gameObject.SetActive(true);
        hyperjump.Play();
        hyperjump.pause = true;
    }
}
