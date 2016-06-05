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
	KeyCode comboKey = KeyCode.None;

	// Use this for initialization
	void Start () {
		if (button == null || button.onClick == null) {
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (inputKey)) {
			if (comboKey == KeyCode.None || Input.GetKey (comboKey)) {
				//ExecuteEvents.Execute<IPointerClickHandler> (gameObject, new BaseEventData (EventSystem.current), ExecuteEvents.pointerClickHandler);
				ExecuteEvents.Execute<ISubmitHandler> (gameObject, new BaseEventData (EventSystem.current), ExecuteEvents.submitHandler);
			}
		}
	}

	void Reset(){
		button = GetComponent<Button> ();
	}
}
