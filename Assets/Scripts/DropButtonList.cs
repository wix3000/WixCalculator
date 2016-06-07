using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace UnityEngine.UI{
	public class DropButtonList : MonoBehaviour, ISelectHandler, IDeselectHandler {

		[SerializeField]
		GameObject menuItem;

		// Use this for initialization
		void Start () {

		}

		void Reset(){
			menuItem = transform.Find ("MenuItem").gameObject;
		}
			
		public void OnSelect (BaseEventData eventData){
			menuItem.SetActive (true);
		}

		public void OnDeselect (BaseEventData eventData){
			StartCoroutine (Close ());
		}

		IEnumerator Close(){
			yield return null;
			menuItem.SetActive (false);
		}

	}
}