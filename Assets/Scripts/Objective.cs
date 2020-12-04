using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : Interactible
{
    private AudioSource _audioSource;
    //NEEDS CONNECTION TO UI HANDLER TO OPEN UP MINI GAME UI

    public void Start()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();
    }

    public override void Interact()
    {
        base.Interact();

        //CALL MINI GAME UI

        _audioSource.Play();
    }

}
