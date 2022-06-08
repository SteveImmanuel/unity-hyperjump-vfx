using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXController : MonoBehaviour
{
    public VisualEffect hyperjump;

    private float elapsed;
    private float cooldown;
    private bool isPlaying;

    private void Awake()
    {
        isPlaying = false;
        elapsed = 0f;
        cooldown = hyperjump.GetFloat("BeamLifetime");
    }

    void Start()
    {
        ResetVFX();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isPlaying)
        {
            isPlaying = true;
            hyperjump.pause = false;
            elapsed = 0f;
        }

        if (elapsed > cooldown + 1f && isPlaying)
        {
            elapsed = 0;
            isPlaying = false;
            ResetVFX();
        }

        elapsed += Time.deltaTime;
    }

    private void ResetVFX()
    {
        hyperjump.gameObject.SetActive(false);
        hyperjump.gameObject.SetActive(true);
        hyperjump.Play();
        hyperjump.pause = true;
    }

    private void Invert(bool invert)
    {
        hyperjump.SetBool("Invert", invert);
    }
}
