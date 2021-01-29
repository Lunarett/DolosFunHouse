using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerInstantiationHelper : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    [SerializeField] private PlayerController playerController;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        playerController.OnPhotonInstantiate(info);
    }
}
