using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHandler : MonoBehaviour
{
    [SerializeField] private Image _normalCrossHair;
    [SerializeField] private Image _selectedCrossHair;

    public void SwitchCrosshair()
    {
        if (_normalCrossHair.gameObject.activeSelf)
        {
            SelectedCrosshair();
        }
        else
        {
            NormalCrosshair();
        }
    }

    public void SelectedCrosshair()
    {
        _normalCrossHair.gameObject.SetActive(false);
        _selectedCrossHair.gameObject.SetActive(true);
    }

    public void NormalCrosshair()
    {
        _normalCrossHair.gameObject.SetActive(true);
        _selectedCrossHair.gameObject.SetActive(false);
    }
}
