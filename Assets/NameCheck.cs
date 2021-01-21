using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class NameCheck : MonoBehaviour
{
	[SerializeField] TMP_InputField inputName;
	[SerializeField] TMP_Text description;

	[SerializeField] UnityEvent proceedToMenu;

	public void Check()
	{
		if(string.IsNullOrWhiteSpace(inputName.text))
		{
			description.text = "Please Enter a valid name!";
		}
		else
		{
			proceedToMenu.Invoke();
		}
	}
}
