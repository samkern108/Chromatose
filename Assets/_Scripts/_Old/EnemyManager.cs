using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class EnemyManager : MonoBehaviour, IRestartObserver, IPlayerObserver {

	[EventID]
	public string eventID;

	public static EnemyManager self;
	public GameObject p_enemy;
	public Room room;

	public void Start() {
		Koreographer.Instance.RegisterForEvents(eventID, SpawnEnemyEvent);

		self = this;
		NotificationMaster.restartObservers.Add (this);
		NotificationMaster.playerObservers.Add (this);
	}

	private void SpawnEnemyEvent(KoreographyEvent evt) {
		GameObject.Instantiate (p_enemy, this.transform);
	}

	private static int _numEnemies;
	public int NumEnemies
	{
		get { return transform.childCount; }
	}

	public void Restart() {
		DestroyAllChildren ();
	}

	public void PlayerDied() {
		DestroyAllChildren ();
	}

	private void DestroyAllChildren() {
		while (transform.childCount > 0) {
			Transform child = transform.GetChild (0);
			child.parent = null;
			Destroy (child.gameObject);
		}
	}
}
