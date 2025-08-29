using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreditsView : UiView
{
	private void OnEnable()
	{
		EventSystem.current.SetSelectedGameObject(GetBackButton().gameObject);
	}
}
