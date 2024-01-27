using UnityEngine;
using UnityEngine.UI;

public class MoodIndicatorView : MonoBehaviour
{
    [SerializeField] private Image _indicator;
    [SerializeField] private Image _moodIcon;
    private MoodIndicator _moodIndicator;

    public void Construct(MoodIndicator moodIndicator)
    {
        _moodIndicator = moodIndicator;
    }

    private void OnDisable()
    {
    }

    private void OnUpdateTimerView(float value)
    {
        _indicator.fillAmount = value;
    }
    
}