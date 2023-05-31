using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;


public class LightModel : MonoBehaviour
{
    [SerializeField] MeshRenderer elsRenderer;
    [SerializeField] MeshRenderer lightRenderer;
    Material elsMaterial;
    Material lightMaterial;
    [SerializeField] List<Light> lights;
    CancellationTokenSource cts;

    private void Awake()
    {
        elsMaterial = elsRenderer.material;
        lightMaterial = lightRenderer.material;
    }
    public async void LightFlicker(int lightCount)
    {
        Debug.Log("DoIntensity 시작 ");

        ColorUtility.TryParseHtmlString("#02FF00", out Color elsColor);
        ColorUtility.TryParseHtmlString("#000000", out Color lightColor);

        elsMaterial.SetColor("_EmissionColor", elsColor);
        lightMaterial.SetColor("_EmissionColor", lightColor);
        for (int i = 0; i < lightCount; i++)
        {
            cts = new();

            await UniTask.WhenAll(lights.Select(light => light.DOIntensity(1, Random.Range(0.1f, 0.5f)).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine).ToUniTask(cancellationToken: cts.Token)));
            if (cts.IsCancellationRequested) break;
        }
        Debug.Log("DoIntensity 끝 ");
    }
    public void LightEnd()
    {
        Debug.Log("DoKill 시작");
       
        lights.ForEach(light => light.DOKill());
        cts.Cancel();
        Debug.Log("DoKill 끝");
    }
}