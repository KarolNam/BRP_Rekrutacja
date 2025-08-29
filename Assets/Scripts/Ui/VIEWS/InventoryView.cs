using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryView : UiView
{
    [Header("Inventory Elements")] [SerializeField]
    private SoulInformation SoulItemPlaceHolder;

    [SerializeField] private Text Description;
    [SerializeField] private Text Name;
    [SerializeField] private Image Avatar;
    [SerializeField] private Button UseButton;
    [SerializeField] private Button DestroyButton;

    private RectTransform _contentParent;
    private GameObject _currentSelectedGameObject;
    private SoulInformation _currentSoulInformation;
    
    // adding list to navigate through InventoryView
    private readonly List<GameObject> soulGameObjects = new();
    //
    // currently selected button reference
    private GameObject selectedSoulButton;
    //
	public override void Awake()
    {
        base.Awake();
        _contentParent = (RectTransform)SoulItemPlaceHolder.transform.parent;
        InitializeInventoryItems();
    }

    private void InitializeInventoryItems()
    {
        for (int i = 0, j = SoulController.Instance.Souls.Count; i < j; i++)
        {
            SoulInformation newSoul = Instantiate(SoulItemPlaceHolder.gameObject, _contentParent).GetComponent<SoulInformation>();
            newSoul.SetSoulItem(SoulController.Instance.Souls[i], () => SoulItem_OnClick(newSoul));
            // adding objects to list
            soulGameObjects.Add(newSoul.gameObject);
            //
        }

        SoulItemPlaceHolder.gameObject.SetActive(false);
    }
    // setting nav to backButton
    private void SetBackButtonNav()
    {
		var nav = GetBackButton().navigation;
		nav.mode = Navigation.Mode.Explicit;
		nav.selectOnLeft = selectedSoulButton != null ? selectedSoulButton.GetComponent<Button>() : soulGameObjects[0].GetComponent<Button>();
        nav.selectOnDown = DestroyButton.gameObject.activeInHierarchy ? DestroyButton : null;
		GetBackButton().navigation = nav;
	}
    //

    private void OnEnable()
    {
        ClearSoulInformation();
        // setting first soul when enable and backButton navigation
        SelectElement(0);
        _currentSelectedGameObject = soulGameObjects[0];
        SetBackButtonNav();
        //
    }

	private void OnDisable()
	{
        if (EventSystem.current.currentSelectedGameObject != null) EventSystem.current.SetSelectedGameObject(null);
        _currentSelectedGameObject = null;
	}

    // update scroll when selecting
	private void Update()
	{
		if (EventSystem.current.currentSelectedGameObject != UseButton.gameObject && EventSystem.current.currentSelectedGameObject != DestroyButton.gameObject && EventSystem.current.currentSelectedGameObject != GetBackButton().gameObject && EventSystem.current.currentSelectedGameObject != selectedSoulButton)
        {
			selectedSoulButton = EventSystem.current.currentSelectedGameObject;
            SetBackButtonNav();
			ScrollToSelected(soulGameObjects.IndexOf(selectedSoulButton));
        }
	}

    private void ScrollToSelected(int index)
    {
        if (index < 12) _contentParent.anchoredPosition = new Vector2(0, 0);
        if (index >= 12) _contentParent.anchoredPosition = new Vector2(0, 200);
        if (index >= 15) _contentParent.anchoredPosition = new Vector2(0, 390);
        if (index >= 18) _contentParent.anchoredPosition = new Vector2(0, 560);
	}
    //

	private void ClearSoulInformation()
    {
        Description.text = "";
        Name.text = "";
        Avatar.sprite = null;
        SetupUseButton(false);
        SetupDestroyButton(false);
        _currentSelectedGameObject = null;
        _currentSoulInformation = null;
    }

    public void SoulItem_OnClick(SoulInformation soulInformation)
    {
        _currentSoulInformation = soulInformation;
        _currentSelectedGameObject = soulInformation.gameObject;
        SetupSoulInformation(soulInformation.soulItem);
    }

    private void SetupSoulInformation(SoulItem soulItem)
    {
        Description.text = soulItem.Description;
        Name.text = soulItem.Name;
        Avatar.sprite = soulItem.Avatar;
        SetupUseButton(soulItem.CanBeUsed);
        SetupDestroyButton(soulItem.CanBeDestroyed);
    }

    private void SelectElement(int index)
    {
        EventSystem.current.SetSelectedGameObject(soulGameObjects[index]);
    }

    private void CantUseCurrentSoul()
    {
        PopUpInformation popUpInfo = new PopUpInformation { DisableOnConfirm = true, UseOneButton = true, Header = "CAN'T USE", Message = "THIS SOUL CANNOT BE USED IN THIS LOCALIZATION" };
        GUIController.Instance.ShowPopUpMessage(popUpInfo);
    }

    private void UseCurrentSoul(bool canUse)
    {
        if (!canUse)
        {
            CantUseCurrentSoul();
        }
        else
        {
            //USE SOUL
            soulGameObjects.Remove(_currentSelectedGameObject);
            Destroy(_currentSelectedGameObject);
            ClearSoulInformation();
            // set back to first soul and update BackButton navigation
			SelectElement(0);
			SetBackButtonNav();
            //
			// granting random amount of souls when soul is used
			int soulsReward = Random.Range(20, 101);
            GUIController.Instance.EarnSouls(soulsReward);
            //
        }
    }

    private void DestroyCurrentSoul()
    {
        // removing soul from list
		soulGameObjects.Remove(_currentSelectedGameObject);
        //
		Destroy(_currentSelectedGameObject);
        ClearSoulInformation();
		// set back to first soul and update BackButton navigation
		SelectElement(0);
		SetBackButtonNav();
        //
	}

    private void SetupUseButton(bool active)
    {
        UseButton.onClick.RemoveAllListeners();
        if (active)
        {
            bool isInCorrectLocalization = GameControlller.Instance.IsCurrentLocalization(_currentSoulInformation.soulItem.UsableInLocalization);
            // disabling interacting with usebutton if not in correct location
			if (!isInCorrectLocalization)
			{
				UseButton.interactable = false;
				UseButton.gameObject.SetActive(active);
				return;
			}
			UseButton.interactable = true;
            //
            // selecting UseButton if it's interactable after selected soul and setting nav
            EventSystem.current.SetSelectedGameObject(UseButton.gameObject);
			var nav = UseButton.navigation;
			nav.mode = Navigation.Mode.Explicit;
			nav.selectOnLeft = _currentSelectedGameObject.GetComponent<Button>();
            nav.selectOnRight = DestroyButton;
            nav.selectOnUp = GetBackButton();
			UseButton.navigation = nav;
			//
			PopUpInformation popUpInfo = new PopUpInformation
            {
                DisableOnConfirm = isInCorrectLocalization,
                UseOneButton = false,
                Header = "USE ITEM",
                Message = "Are you sure you want to USE: " + _currentSoulInformation.soulItem.Name + " ?",
                Confirm_OnClick = () => UseCurrentSoul(isInCorrectLocalization),
                // reference to select UseButton on back
				SenderReference = UseButton.gameObject
                //
			};
            UseButton.onClick.AddListener(() => GUIController.Instance.ShowPopUpMessage(popUpInfo));
        }
        UseButton.gameObject.SetActive(active);
    }

    private void SetupDestroyButton(bool active)
    {
        DestroyButton.onClick.RemoveAllListeners();
        if (active)
        {
			// selecting DestroyButton if UseButton is not interactable and setting nav
			if (!UseButton.interactable)
			{
				EventSystem.current.SetSelectedGameObject(DestroyButton.gameObject);
				var nav = DestroyButton.navigation;
				nav.mode = Navigation.Mode.Explicit;
				nav.selectOnLeft = _currentSelectedGameObject.GetComponent<Button>();
				nav.selectOnUp = GetBackButton();
				DestroyButton.navigation = nav;
			}
			else
			{
				var nav = DestroyButton.navigation;
				nav.mode = Navigation.Mode.Explicit;
				nav.selectOnLeft = UseButton;
				nav.selectOnUp = GetBackButton();
				DestroyButton.navigation = nav;
			}
            //
            PopUpInformation popUpInfo = new PopUpInformation
            {
                DisableOnConfirm = true,
                UseOneButton = false,
                Header = "DESTROY ITEM",
                Message = "Are you sure you want to DESTROY: " + Name.text + " ?",
                Confirm_OnClick = () => DestroyCurrentSoul(),
				// reference to select DestroyButton on back
				SenderReference = DestroyButton.gameObject
                //
            };
            DestroyButton.onClick.AddListener(() => GUIController.Instance.ShowPopUpMessage(popUpInfo));
        }

        DestroyButton.gameObject.SetActive(active);
        // setting back button nav
        SetBackButtonNav();
        //
    }
}