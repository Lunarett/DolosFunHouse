using UnityEngine;
using TMPro;

namespace Michsky.UI.Dark
{
	public class CreateLobby : MonoBehaviour
	{
		[Header("Modals")]
		[SerializeField] private ModalWindowManager _modalWindowError;
		[SerializeField] private ModalWindowManager _modalWindowConfirm;
		[Space]
		[SerializeField] private BlurManager _blurManager;
		[Space]
		public TMP_InputField TitleText;
		public TMP_InputField DescText;

		public void OpenWindow()
		{
			if (string.IsNullOrWhiteSpace(TitleText.text))
			{
				_modalWindowError.ModalWindowIn();
			}
			else
			{
				_modalWindowConfirm.ModalWindowIn();
			}
		}
	}
}
