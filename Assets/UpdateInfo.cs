using UnityEngine;
using TMPro;

namespace Michsky.UI.Dark
{
	public class UpdateInfo : MonoBehaviour
	{
		[SerializeField] private CreateLobby _createLobby;
		[Space]
		[SerializeField] private TextMeshProUGUI _title;
		[SerializeField] private TextMeshProUGUI _desc;

		private void Start()
		{
			_title.text = _createLobby.TitleText.text;
			_desc.text = _createLobby.DescText.text;
		}
	}
}
