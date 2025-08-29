using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReadMeView : UiView
{
	private void OnEnable()
	{
		EventSystem.current.SetSelectedGameObject(GetBackButton().gameObject);
	}
}
