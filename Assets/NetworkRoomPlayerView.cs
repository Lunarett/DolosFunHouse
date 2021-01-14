using UnityEngine;
using TMPro;
using Photon.Realtime;

public class NetworkRoomPlayerView : MonoBehaviour
{
	[SerializeField] private TMP_Text _name;
	[SerializeField] private GameObject _you;
	[SerializeField] private GameObject _host;

	public void Setup(Player player, TMP_InputField nickName)
	{
		_name.text = nickName.text;
		player.NickName = _name.text;
		_you.SetActive(player.IsLocal);
		_host.SetActive(player.IsMasterClient);
	}
}
