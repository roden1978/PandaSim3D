using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Infrastructure;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Curtain : MonoBehaviour
{
    [SerializeField] private CanvasGroup _fader;
    [SerializeField] private Image _slider;
    private IPersistentProgress _progress;
    private float _count;
    private ISceneLoader _sceneLoader;

    [Inject]
    public void Construct(IPersistentProgress progress, ISceneLoader sceneLoader)
    {
        _progress = progress;
        _sceneLoader = sceneLoader;
    }

    private void Start()
    {
        Show();
    }

    private IEnumerator Fill()
    {
        while (_count < 1)
        {
            _count += Time.deltaTime;
            _slider.fillAmount = _count;
            SetColour(_count);
            yield return null;
        }

        var result = _sceneLoader.LoadScene(_progress.PlayerProgress.PlayerState.SceneName);
        while (result.Status != UniTaskStatus.Succeeded)
            yield return null;
    }

    public void Hide() =>
        StartCoroutine(Fill());

    private void Show()
    {
        StartCoroutine(Fill());
    }

    private void SetColour(float current)
    {
        float twoThirds = Convert.ToSingle(1) / 3 * 2 / 1;
        float oneThird = Convert.ToSingle(1) / 3 / 1;
        _slider.color = current < twoThirds && current > oneThird ? Color.yellow :
            current < oneThird ? Color.red : Color.green;
    }
}