using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MoodIndicatorView : MonoBehaviour
{
    [SerializeField] private Slider _indicator;
    [SerializeField] private Image _fillArea;
    [SerializeField] private Image _moodIcon;
    
    private readonly Color[] _colors = { new(1, 0, 0), new(1, .3f, 0), Color.yellow, Color.green };
    private MoodIndicator _moodIndicator;

    public void Construct(MoodIndicator moodIndicator)
    {
        _moodIndicator = moodIndicator;
        _indicator.onValueChanged.AddListener(SetColour);
    }
    public void Initialize()
    {
        SetColour(1); //TODO: Change after indicator state loading
        _moodIndicator.UpdateIndicatorValue += OnUpdateIndicatorView;
    }

    private void OnDisable()
    {
        _moodIndicator.UpdateIndicatorValue -= OnUpdateIndicatorView;
        _moodIndicator.Dispose();
    }

    private void OnUpdateIndicatorView(float value)
    {
        _indicator.value = value;
        SetColour(value);
    }

    private void SetColour(float current)
    {
        (Color oldColor, Color newColor, float newT) result = Calculate(current);
        _fillArea.color = _moodIcon.color = Color.Lerp(result.oldColor, result.newColor, result.newT);
    }

    private (Color, Color, float) Calculate(float current)
    {
        float scaledTime = current * (_colors.Length - 1);
        Color oldColor = _colors[(int)scaledTime];
        Color newColor = current >= 1 ? _colors[^1] : _colors[(int)(scaledTime + 1f)];
        float newT = scaledTime - Mathf.Floor(scaledTime);

        return (oldColor, newColor, newT);
    }

}