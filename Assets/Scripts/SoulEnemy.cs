using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoulEnemy : MonoBehaviour, IEnemy
{
    [SerializeField] private GameObject InteractionPanelObject;
    [SerializeField] private GameObject ActionsPanelObject;
    [SerializeField] private SpriteRenderer EnemySpriteRenderer;
	// susceptibility & soulsReward
	public string Susceptibility { get; set; }
    private int soulsReward = 10;
	//
    // spawn point index for clearing combat button when combat is over
	private int spawnPointIndex;
    //

	private SpawnPoint _enemyPosition;

    public void SetupEnemy(Sprite sprite, SpawnPoint spawnPoint, int index)
    {
        EnemySpriteRenderer.sprite = sprite;
        _enemyPosition = spawnPoint;
        spawnPointIndex = index;
        gameObject.SetActive(true);
        // setting button with index of enemy spawn point in GUIController to set navigation
        GUIController.Instance.SetEnemyCombatButton(InteractionPanelObject.GetComponentInChildren<Button>(), spawnPointIndex);
	}

    public SpawnPoint GetEnemyPosition()
    {
        return _enemyPosition;
    }

    public GameObject GetEnemyObject()
    {
        return this.gameObject;
    }

    private void ActiveCombatWithEnemy()
    {
        ActiveInteractionPanel(false);
        ActiveActionPanel(true);
        // Selecting first combat option button (bow)
		EventSystem.current.SetSelectedGameObject(ActionsPanelObject.GetComponentInChildren<Button>().gameObject);
        //
	}

    private void ActiveInteractionPanel(bool active)
    {
        InteractionPanelObject.SetActive(active);
    }

    private void ActiveActionPanel(bool active)
    {
        ActionsPanelObject.SetActive(active);
    }

    private void UseBow()
    {
        // USE BOW
        GameEvents.EnemyKilled?.Invoke(this);
		// granting souls w/ & w/o susceptibility
        if (Susceptibility == "Distance")
			GUIController.Instance.EarnSouls(Mathf.RoundToInt(soulsReward * 1.5f));
        else
		    GUIController.Instance.EarnSouls(soulsReward);
		//
		// clearing combat button at index
		GUIController.Instance.ClearEnemyCombatButton(spawnPointIndex);
        //
	}

    private void UseSword()
    {
        GameEvents.EnemyKilled?.Invoke(this);
		// USE SWORD
		// granting souls w/ & w/o susceptibility
		if (Susceptibility == "Melee")
			GUIController.Instance.EarnSouls(Mathf.RoundToInt(soulsReward * 1.5f));
		else
			GUIController.Instance.EarnSouls(soulsReward);
		//
		// clearing combat button at index
		GUIController.Instance.ClearEnemyCombatButton(spawnPointIndex);
		//
	}

	#region OnClicks

	public void Combat_OnClick()
    {
        ActiveCombatWithEnemy();
    }

    public void Bow_OnClick()
    {
        UseBow();
    }

    public void Sword_OnClick()
    {
        UseSword();
    }

    #endregion
}


public interface IEnemy
{
    SpawnPoint GetEnemyPosition();
    GameObject GetEnemyObject();
}
