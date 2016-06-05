using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using Wix.Calculator;

public class CalculatorInterface : MonoBehaviour {

	[SerializeField]
	Text inputLine;

	List<Element> elements = new List<Element> ();
	Element result;

	public CalculatorMode mode = CalculatorMode.Classis;

    void Start() {
		RefreshInputLine ();
    }

	void Update(){
		if(Input.GetKeyDown(KeyCode.P)){
			Calculator c = new Calculator ();
			c.PreFix (elements);
			RefreshInputLine (c.equation);
		}
	}

    void PushValue(ValueElement e) {
		if (result is Error) {
			return;
		}
			
		if (elements.Count == 0) {
			elements.Add (e);
		} else {
			Element last = elements.Last ();
			if (last is ValueElement) {
				((ValueElement)last).Link (e);
			} else {
				elements.Add (e);
			}
		}
		RefreshInputLine();
    }

	void PushOperator(OperatorElement ope){
		if (result is Error) {
			return;
		}

		if (elements.Count == 0) {
			if (result != null) {
				elements.Add (result);
			}
		}
		elements.Add (ope);

		RefreshInputLine ();
	}

	void RefreshInputLine(Element element) {
		print ((element.ToString ().Length > 10));
		inputLine.text = (element.ToString ().Length > 10) ? ((ValueElement)element).ToString ("E5") : element.ToString ();
	}

	void RefreshInputLine(List<Element> datas = null) {
		if (datas == null)
			datas = elements;

		StringBuilder sb = new StringBuilder ();
		foreach (Element e in datas) {
			sb.Append (e.ToString ());
		}
		inputLine.text = sb.ToString ();
    }

	void RunCalculator(){
		if (elements.Last () is ValueElement) {
			((ValueElement)elements.Last ()).FinishEdit ();
		}
		Calculator c = new Calculator (elements);
		result = c.result;
		elements.Clear ();
		RefreshInputLine (result);
	}

	public void OnNumbicClick(int numbic){
		PushValue (new ValueElement (numbic));
	}

	public void OnDotClick(){
		if (elements.Count > 0) {
			(elements.Last () as ValueElement)?.Dot ();
		}
		RefreshInputLine ();
	}

	public void OnSighClick(){
		if (elements.Count > 0) {
			(elements.Last () as ValueElement)?.Sigh ();
		}
		RefreshInputLine ();
	}

	public void OnSqrtClick(){
		PushOperator (new Sqrt ());
	}

	public void OnSquareClick(){
		PushOperator (new Square ());
	}

	public void OnAddClick(){
		PushOperator (new Add ());
	}

	public void OnSubtractClick(){
		PushOperator (new Subtract ());
	}

	public void OnMultiplyClick(){
		PushOperator (new Multiply ());
	}

	public void OnDivideClick(){
		PushOperator (new Divide ());
	}
		
	public void OnOparenClick(){
		PushOperator (new Oparen ());
	}

	public void OnCparenClick(){
		PushOperator (new Cparen ());
	}

	public void OnEqualClick(){
		RunCalculator ();
	}

	public void OnClearClick(){
		elements.Clear ();
		result = null;
		RefreshInputLine ();
	}

	public void OnBackspaceClick(){
		if (elements.Count == 0) {
			result = null;
			RefreshInputLine ();
			return;
		}

		Element e = elements.Last ();

		if (e is OperatorElement || e.ToString ().Length <= 1) {
			elements.RemoveAt (elements.Count - 1);
		} else {
			((ValueElement)e).Backspace ();
		}

		RefreshInputLine ();
	}

    public enum CalculatorMode {
        Classis,
        Advance
    }

	#region 進階函數按鍵

	void PushAlgebra(Element e){
		if (result is Error) {
			return;
		}

		if (elements.Count == 0) {
			elements.Add (e);
		} else {
			Element last = elements.Last ();
			if (last is ValueElement) {
				return;
			} else {
				elements.Add (e);
			}
		}
		RefreshInputLine();
	}

	public void OnRandClick(){
		PushAlgebra (new Wix.Calculator.Random ());
	}

	public void OnEClick(){
		PushAlgebra (new MathematicalConstant ());
	}

	public void OnExponentialClick(){
		PushOperator (new Exponential ());
	}

	public void OnFactorialClick(){
		PushOperator (new Factorial ());
	}

	#endregion
}

