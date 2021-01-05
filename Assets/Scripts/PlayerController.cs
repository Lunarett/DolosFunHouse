using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Material highlightMaterial;
    private Material defaultMaterial;

    private Renderer _selectionRenderer;
    private Transform _selection;
    [SerializeField] private float _maxInsteractDistance = 2f;

    //[SerializeField] private UIHandler _UIHandler;

    [SerializeField] Camera _playerCam;

    UnityEvent _interactEvent = new UnityEvent();

    private void Update()
    {

        if (_playerCam.gameObject.activeSelf)
        {
            //object selection based on this https://www.youtube.com/watch?v=_yf5vzZ2sYE&ab_channel=InfallibleCode
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Ray ray = new Ray(transform.position, _playerCam.transform.forward * _maxInsteractDistance);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.distance <= _maxInsteractDistance)
                {
                    if (hit.transform.CompareTag("Selectable"))
                    {
                        //show UI
                        //_UIHandler.ShowInteractText();

                        //we've got a selectable and it's different from the one we already have
                        if (hit.transform != _selection)
                        {
                            RemoveHighlight();
                            ApplyHighlight(hit);
                        }

                        Interactible interactible;

                        if (((interactible = _selection.GetComponent<Interactible>()) != null) && Input.GetKeyDown(KeyCode.E))
                        {

                            interactible.Interact(gameObject);
                        }
                    }

                    else
                    {
                        RemoveHighlight();
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            _interactEvent.Invoke();
                            Debug.Log("Invoked");
                        }
                    }
                }
            }
        }

        else
        {
            RemoveHighlight();
            if (Input.GetKeyDown(KeyCode.E))
            {
                _interactEvent.Invoke();
                Debug.Log("Invoked");
            }
        }
    }

    private void ApplyHighlight(RaycastHit hit)
    {
        _selection = hit.transform;

        _selectionRenderer = _selection.GetComponent<Renderer>();

        if (_selectionRenderer != null)
        {
            defaultMaterial = _selectionRenderer.material;
            _selectionRenderer.material = highlightMaterial;
        }
    }

    private void RemoveHighlight()
    {
        if (_selection != null)
        {
            _selectionRenderer.material = defaultMaterial;
            _selection = null;
            _selectionRenderer = null;
            //_UIHandler.HideInteractText();
        }
    }

    public void ToggleCharacterActive()
    {
        if (GetComponent<CharacterController>().enabled)
        {
            GetComponent<CharacterController>().enabled = false;
        }
        else
        {
            GetComponent<CharacterController>().enabled = true;
        }
    }
    public UnityEvent GetInteractEvent()
    {
        return _interactEvent;
    }

    public void SwitchActiveCam(Camera otherCam)
    {
        _playerCam.gameObject.SetActive(!_playerCam.gameObject.activeSelf);
        otherCam.gameObject.SetActive(!otherCam.gameObject.activeSelf);
    }
}
