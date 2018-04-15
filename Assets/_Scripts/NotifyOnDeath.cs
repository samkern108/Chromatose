using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INotifyOnDeathObserver
{
	void NotifyOnDeath(bool progressImmediate);
}
public class NotifyOnDeath : MonoBehaviour {

	public bool progressImmediate = false;

	public INotifyOnDeathObserver notifyDelegate;

	void OnDestroy () {
		notifyDelegate.NotifyOnDeath(progressImmediate);
	}
}
