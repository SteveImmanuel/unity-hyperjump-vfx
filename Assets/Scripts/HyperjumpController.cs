using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;

public class HyperjumpController : MonoBehaviour
{
    public VolumeProfile globalVolume;
    public VisualEffect hyperjump;
    public Renderer tunnelRenderer;
    public ShipController shipController;
    
    [Header("Transition Settings")]
    public float transitionDuration;

    [Header("Beam Settings")]
    public float idleScale = 0f;
    public float maxScale = 400f;
    public AnimationCurve beamCurve;

    [Header("Lightspeed Tunnel Settings")]
    public float transitionRadialScale = 0.5f;
    public float idleRadialScale = 1.2f;
    public float alphaMin = -1f;
    public float alphaMax = 1f;
    public float transitionWarpSpeed = 0.4f;
    public float idleWarpSpeed = 1.5f;
    public AnimationCurve tunnelCurve;

    [Header("Postprocessing Settings")]
    public float brightestBloom = 60f;
    public float normalBloom = 1.5f;

    private bool inTunnel;
    private bool isTransitioning;
    private Bloom bloom;

    private void Awake()
    {
        inTunnel = false;
        isTransitioning = false;
    }

    void Start()
    {
        globalVolume.TryGet(out bloom);
        ResetVFX();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTransitioning)
        {

            if (!inTunnel)
            {
                isTransitioning = true;
                shipController.enabled = false;
                StartCoroutine(EnterTunnelPhase1(() => {
                    StartCoroutine(EnterTunnelPhase2(() =>
                    {
                        inTunnel = true;
                        isTransitioning = false;
                        ResetVFX();
                    }));
                }));
            } 
            else
            {
                isTransitioning = true;
                StartCoroutine(ExitTunnelPhase1(() => {
                    StartCoroutine(ExitTunnelPhase2(() => {
                        inTunnel = false;
                        isTransitioning = false;
                        shipController.enabled = true;
                    }));
                }));
            }
        }
    }

    private void ResetVFX()
    {
        hyperjump.gameObject.SetActive(false);
        hyperjump.gameObject.SetActive(true);
    }

    // This IEnumerator functions should be able to be optimized into one or two functions, however I don't have time now

    private IEnumerator EnterTunnelPhase1(Action callback = null)
    {
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            float frac = elapsed / transitionDuration;
            float tBeam = beamCurve.Evaluate(frac);
            float tTunnel = tunnelCurve.Evaluate(frac);

            hyperjump.SetFloat("LinearScale", Mathf.Lerp(idleScale, maxScale, tBeam));
            bloom.intensity.value = Mathf.Lerp(normalBloom, brightestBloom, tBeam);
            tunnelRenderer.material.SetFloat("_RadialScale", Mathf.Lerp(idleRadialScale, transitionRadialScale, tTunnel));
            tunnelRenderer.material.SetFloat("_AlphaSlider", Mathf.Lerp(alphaMin, alphaMax, tTunnel));

            elapsed += Time.deltaTime;
            yield return null;
        }

        hyperjump.SetFloat("LinearScale", maxScale);
        bloom.intensity.value = brightestBloom;
        tunnelRenderer.material.SetFloat("_RadialScale", transitionRadialScale);
        tunnelRenderer.material.SetFloat("_AlphaSlider", alphaMax);

        callback?.Invoke();
    }

    private IEnumerator EnterTunnelPhase2(Action callback = null)
    {
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            float frac = elapsed / transitionDuration;
            float tBeam = 1 - beamCurve.Evaluate(1 - frac);
            float tTunnel = 1 - tunnelCurve.Evaluate(1 - frac);

            bloom.intensity.value = Mathf.Lerp(brightestBloom, normalBloom, tBeam);
            tunnelRenderer.material.SetFloat("_RadialScale", Mathf.Lerp(transitionRadialScale, idleRadialScale, tTunnel));

            elapsed += Time.deltaTime;
            yield return null;
        }

        bloom.intensity.value = normalBloom;
        tunnelRenderer.material.SetFloat("_RadialScale", idleRadialScale);

        callback?.Invoke();
    }

    private IEnumerator ExitTunnelPhase1(Action callback = null)
    {
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            float frac = elapsed / transitionDuration;
            float tBeam = beamCurve.Evaluate(frac);
            float tTunnel = tunnelCurve.Evaluate(frac);

            bloom.intensity.value = Mathf.Lerp(normalBloom, brightestBloom, tBeam);
            tunnelRenderer.material.SetFloat("_RadialScale", Mathf.Lerp(idleRadialScale, transitionRadialScale, tTunnel));

            elapsed += Time.deltaTime;
            yield return null;
        }

        bloom.intensity.value = brightestBloom;
        tunnelRenderer.material.SetFloat("_RadialScale", transitionRadialScale);

        callback?.Invoke();
    }

    private IEnumerator ExitTunnelPhase2(Action callback = null)
    {
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            float frac = elapsed / transitionDuration;
            float tBeam = 1 - beamCurve.Evaluate(1 - frac);
            float tTunnel = 1 - tunnelCurve.Evaluate(1 - frac);

            hyperjump.SetFloat("LinearScale", Mathf.Lerp(-maxScale, idleScale, tBeam));
            bloom.intensity.value = Mathf.Lerp(brightestBloom, normalBloom, tBeam);
            tunnelRenderer.material.SetFloat("_RadialScale", Mathf.Lerp(transitionRadialScale, idleRadialScale, tTunnel));
            tunnelRenderer.material.SetFloat("_AlphaSlider", Mathf.Lerp(alphaMax, alphaMin, tTunnel));

            elapsed += Time.deltaTime;
            yield return null;
        }

        hyperjump.SetFloat("LinearScale", idleScale);
        bloom.intensity.value = normalBloom;
        tunnelRenderer.material.SetFloat("_RadialScale", idleRadialScale);
        tunnelRenderer.material.SetFloat("_AlphaSlider", alphaMin);

        callback?.Invoke();
    }
}
