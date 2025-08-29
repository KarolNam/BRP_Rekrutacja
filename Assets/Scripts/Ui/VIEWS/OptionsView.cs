using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsView : UiView
{
	private void OnEnable()
	{
		EventSystem.current.SetSelectedGameObject(gameObject.GetComponentInChildren<Toggle>().gameObject);
	}
}
