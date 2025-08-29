using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{

    #region singleton

    public static GUIController Instance;

    private void Awake()
    {
        DisableOnStartObject.SetActive(false);
        Instance = this;
    }

    #endregion

    [SerializeField] private GameObject DisableOnStartObject;
    [SerializeField] private RectTransform ViewsParent;
    [SerializeField] private GameObject InGameGuiObject;
    [SerializeField] private PopUpView PopUp;
    [SerializeField] private PopUpScreenBlocker ScreenBlocker;
    // souls amount
    [SerializeField] private TextMeshProUGUI soulsAmountText;
    private int soulsAmount = 0;
    //
    // references to pause and inventory buttons to set nav
    [SerializeField] private Button pauseButton, inventoryButton;
    //
    // reference list of active enemies combatButtons to set nav
    private Button[] enemyCombatButtons = new Button[3];
	//

	private void Start()
    {
        if (ScreenBlocker) ScreenBlocker.InitBlocker();
        // setting soulsAmountText
        soulsAmountText.text = soulsAmount.ToString();
        //
    }

	private void Update()
	{
        if (InGameGuiObject.activeInHierarchy && !enemyCombatButtons.Any(btn => btn == null) && EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(enemyCombatButtons[0].gameObject);
	}

	private void ActiveInGameGUI(bool active)
    {
        InGameGuiObject.SetActive(active);
        // disable interaction with enemy buttons when InGameGuiObject is disabled & active when it get back to active with selecting first enemy combat button
        if (active)
        {
            foreach (Button button in enemyCombatButtons)
            {
                button.interactable = true;
            }
            EventSystem.current.SetSelectedGameObject(enemyCombatButtons[0].gameObject);
        }
        else
        {
			foreach (Button button in enemyCombatButtons)
			{
				button.interactable = false;
			}
		}
        //
    }

    public void ShowPopUpMessage(PopUpInformation popUpInfo)
    {
        PopUpView newPopUp = Instantiate(PopUp, ViewsParent) as PopUpView;
        newPopUp.ActivePopUpView(popUpInfo);
    }

    public void ActiveScreenBlocker(bool active, PopUpView popUpView)
    {
        if (active) ScreenBlocker.AddPopUpView(popUpView);
        else ScreenBlocker.RemovePopUpView(popUpView);
    }
    // earning souls
    public void EarnSouls(int amount)
    {
        soulsAmount += amount;
        soulsAmountText.text = soulsAmount.ToString();
    }
    //
    // setting enemy combat button
    public void SetEnemyCombatButton(Button button, int slotIndex)
    {
        enemyCombatButtons[slotIndex] = button;
		if (EventSystem.current.currentSelectedGameObject == null) EventSystem.current.SetSelectedGameObject(button.gameObject);
		UpdateNav();
    }
    //
    // clearing enemy combat button
    public void ClearEnemyCombatButton(int slotIndex)
    {
        enemyCombatButtons[slotIndex] = null;
        UpdateNav();
    }
    //
    // updating navigation of buttons
    private void UpdateNav()
    {
		var nav = pauseButton.navigation;
		nav.mode = Navigation.Mode.Explicit;
		nav.selectOnRight = enemyCombatButtons[0];
		nav.selectOnDown = enemyCombatButtons[0];
		pauseButton.navigation = nav;

		for (int i = 0; i < enemyCombatButtons.Length; i++)
		{
			Button btn = enemyCombatButtons[i];
			if (btn == null) continue;

			var enemyNav = btn.navigation;
			enemyNav.mode = Navigation.Mode.Explicit;

            if (i == 0)
			    enemyNav.selectOnUp = pauseButton;
            else
				enemyNav.selectOnUp = inventoryButton;

			enemyNav.selectOnLeft = (i > 0 && enemyCombatButtons[i - 1] != null) ? enemyCombatButtons[i - 1] : pauseButton;
            enemyNav.selectOnRight = (i < enemyCombatButtons.Length - 1 && enemyCombatButtons[i + 1] != null) ? enemyCombatButtons[i + 1] : inventoryButton;
			enemyCombatButtons[i].navigation = enemyNav;
		}

		nav = inventoryButton.navigation;
		nav.mode = Navigation.Mode.Explicit;
		nav.selectOnLeft = enemyCombatButtons[2];
		nav.selectOnDown = enemyCombatButtons[2];
		inventoryButton.navigation = nav;
	}
    //
	#region IN GAME GUI Clicks

	public void InGameGUIButton_OnClick(UiView viewToActive)
    {
        viewToActive.ActiveView(() => ActiveInGameGUI(true));

        ActiveInGameGUI(false);
        GameControlller.Instance.IsPaused = true;
    }

    public void ButtonQuit()
    {
        Application.Quit();
    }
    
    #endregion
}