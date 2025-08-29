using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseView : UiView
{
	private void OnEnable()
	{
		EventSystem.current.SetSelectedGameObject(gameObject.GetComponentInChildren<Button>().gameObject);
	}
}
