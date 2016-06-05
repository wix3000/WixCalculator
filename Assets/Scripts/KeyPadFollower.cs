using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class KeyPadFollower : MonoBehaviour {

	[SerializeField]
	Button button;
	[SerializeField]
	KeyCode inputKey;
    [SerializeField]
    bool withCtrl;

	// Use this for initialization
	void Start () {
		if (button == null || button.onClick == null) {
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (inputKey)) {
            if (withCtrl && !(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))) return;

            ExecuteEvents.Execute(gameObject, new BaseEventData (EventSystem.current), ExecuteEvents.submitHandler);
		}
	}

	void Reset(){
		button = GetComponent<Button> ();
	}
}
