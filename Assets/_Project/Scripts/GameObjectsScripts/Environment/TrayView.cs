using GameObjectsScripts;
using UnityEngine;
using Zenject;

public class TrayView : MonoBehaviour
{
    private Tray _tray;
    private IShowable _poop;

    [Inject]
    public void Construct(Tray tray, IShowable poop)
    {
        _tray = tray;
        _poop = poop;
        _tray.ShowPoop += OnShowPoop;
        _tray.HidePoop += OnHidePoop;
    }

    private void OnDisable()
    {
        _tray.Dispose();
        _tray.ShowPoop -= OnShowPoop;
        _tray.HidePoop -= OnHidePoop;
    }

    private void OnShowPoop()
    {
        _poop.Show();
    }

    private void OnHidePoop()
    {
        _poop.Hide();
    }
}