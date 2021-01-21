using UnityEngine;
using TMPro;
using Photon.Realtime;

public class NetworkRoomPlayerView : MonoBehaviour
{
	[SerializeField] private TMP_Text _name;
	[SerializeField] private GameObject _you;
	[SerializeField] private GameObject _host;
	[SerializeField] private GameObject _ready;


	public void Setup(Player player)
	{
		_name.text = player.NickName;

		_you.SetActive(player.IsLocal);
		_host.SetActive(player.IsMasterClient);
		
		if(player.CustomProperties.ContainsKey("Ready"))
			_ready.SetActive((bool)player.CustomProperties["Ready"]);
	}


}
