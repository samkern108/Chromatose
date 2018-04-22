using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagePiece : MonoBehaviour {
	public int stageCounter = 0;

	public void StageWillChange() {
		stageCounter++;
		TransitionToNextStage();
	}

	protected virtual void TransitionToNextStage() {

	}
}
