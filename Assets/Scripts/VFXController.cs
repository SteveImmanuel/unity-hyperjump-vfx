using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;

public class VFXController : MonoBehaviour
{
    public VolumeProfile globalVolume;
    public VisualEffect hyperjump;
    public Renderer tunnelRenderer;
    public AnimationCurve transitionCurve;

    [Header("Lightspeed Tunnel Settings")]
    public float brightestRadialScale = 0.5f;
    public float normalRadialScale = 1.2f;

    [Header("Postprocessing Settings")]
    public float brightestBloom = 60f;
    public float normalBloom = 1.5f;
    public float activeDofStart = 800f;
    public float normalDofStart = 1100f;

    private bool inTunnel;
    private bool isPlaying;
    private Bloom bloom;
    private DepthOfField dof;
    private float transitionDuration;

    private void Awake()
    {
        inTunnel = false;
        isPlaying = false;
    }

    void Start()
    {
        transitionDuration = hyperjump.GetFloat("BeamLifetime");
        globalVolume.TryGet(out bloom);
        globalVolume.TryGet(out dof);
        ResetVFX();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isPlaying)
        {

            if (!inTunnel)
            {
                isPlaying = true;
                hyperjump.pause = false;
                StartCoroutine(FadeInTransition(() => {
                    StartCoroutine(FadeOutTransition(() => { 
                        inTunnel = true;
                        isPlaying = false;
                        ResetVFX();
                        Invert(true);
                    })); 
                }));
            } 
            else
            {
                isPlaying = true;
                StartCoroutine(FadeInTransition(() => {
                    hyperjump.pause = false;
                    StartCoroutine(FadeOutTransition(() => {
                        inTunnel = false;
                        isPlaying = false;
                    }));
                }));
            }
        }
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

    private IEnumerator FadeInTransition(Action callback = null)
    {
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            float frac = elapsed / transitionDuration;
            float t = transitionCurve.Evaluate(frac);

            bloom.intensity.value = Mathf.Lerp(normalBloom, brightestBloom, t);
            tunnelRenderer.material.SetFloat("_RadialScale", Mathf.Lerp(normalRadialScale, brightestRadialScale, t));
            dof.gaussianStart.value = Mathf.Lerp(normalDofStart, activeDofStart, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        bloom.intensity.value = brightestBloom;
        tunnelRenderer.material.SetFloat("_RadialScale", brightestRadialScale);
        dof.gaussianStart.value = activeDofStart;

        tunnelRenderer.gameObject.SetActive(!tunnelRenderer.gameObject.activeInHierarchy);

        callback?.Invoke();
    }

    private IEnumerator FadeOutTransition(Action callback = null)
    {
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            float frac = elapsed / transitionDuration;
            float t = 1 - transitionCurve.Evaluate(1 - frac); ;

            bloom.intensity.value = Mathf.Lerp(brightestBloom, normalBloom, t);
            tunnelRenderer.material.SetFloat("_RadialScale", Mathf.Lerp(brightestRadialScale, normalRadialScale, t));
            dof.gaussianStart.value = Mathf.Lerp(activeDofStart, normalDofStart, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        bloom.intensity.value = normalBloom;
        tunnelRenderer.material.SetFloat("_RadialScale", normalRadialScale);
        dof.gaussianStart.value = normalDofStart;

        callback?.Invoke();
    }
}
