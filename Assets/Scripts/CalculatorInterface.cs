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
	[SerializeField]
	bool isDeg;

	List<Element> elements = new List<Element> ();
	Element result;

	public CalculatorMode mode = CalculatorMode.Classis;
	public bool AutoOparen = false;

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

	void PushElement(Element e){
		if (result is Error)
			return;

		if (e is Algebra) {
			PushAlgebra (e);
		} else if (e is IFunction) {
			PushFunction (e);
		} else if (e is OperatorElement) {
			PushOperator (e);
		} else if (e is ValueElement) {
			PushValue (e);
		}

		result = null;
		RefreshInputLine ();
	}

    void PushValue(Element e) {
		
		if (elements.Count > 0 && elements.Last () is ValueElement) {
			((ValueElement)elements.Last ()).Link ((ValueElement)e);
		} else {
			elements.Add (e);
		}
    }

	void PushOperator(Element ope){
		
		if (result != null) {
			elements.Add (result);
		}

		elements.Add (ope);
	}

	void PushAlgebra(Element e){

		if (elements.Count > 0 && elements.Last () is ValueElement) {
			elements.Add (new Multiply ());
		}
		elements.Add (e);
	}

	void PushFunction(Element e){

		elements.Add (e);
		if (AutoOparen)
			elements.Add (new Oparen ());
		if (result != null)
			elements.Add (result);
	}

	void RefreshInputLine(Element element) {
		inputLine.text = (element.ToString ().Length > 10) ? ((ValueElement)element).ToString ("E7") : element.ToString ();
		//inputLine.text = ((ValueElement)element).ToString("g");
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
		Calculator c = new Calculator (elements, isDeg);
		result = c.result;
		elements.Clear ();
		RefreshInputLine (result);
	}

    #region 基本按鍵區
    public void OnNumbicClick(int numbic){
		PushElement (new ValueElement (numbic));
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
		PushElement (new Sqrt ());
	}

	public void OnSquareClick(){
		PushElement (new Square ());
	}

	public void OnAddClick(){
		PushElement (new Add ());
	}

	public void OnSubtractClick(){
		PushElement (new Subtract ());
	}

	public void OnMultiplyClick(){
		PushElement (new Multiply ());
	}

	public void OnDivideClick(){
		// 按著Ctrl的話就會用÷來顯示
		if (Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl)) {
			PushElement (new Divide2 ());
		} else {
			PushElement (new Divide ());
		}
	}
		
	public void OnOparenClick(){
		PushElement (new Oparen ());
	}

	public void OnCparenClick(){
		PushElement (new Cparen ());
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

    #endregion

    public enum CalculatorMode {
        Classis,
        Advance
    }

	#region 進階函數按鍵



	public void OnRandClick(){
		PushElement(new ValueElement(UnityEngine.Random.value.ToString()));
	}

	public void OnEClick(){
		PushElement(new Algebra(AlgebraType.E));
	}

    public void OnPiClick() {
		PushElement(new Algebra(AlgebraType.Pi));
    }

	public void OnExponentialClick(){
		PushElement (new Exponential ());
	}

	public void OnFactorialClick(){
		PushElement (new Factorial ());
	}

	public void OnCommaClick(){
		PushElement (new Comma ());
	}

	public void OnRootClick(){
		PushElement (new Root ());
	}

    public void OnLogClick(string baseValue) {
        switch (baseValue) {
            case "10":
			PushElement(new Log10());
                break;
            case "2":
			PushElement(new Log2());
                break;
            case "e":
			PushElement(new LogE());
                break;
            default:
			PushElement(new Log());
                break;
        }
    }

    public void OnTrigonometricClick(string type) {
        switch (type) {
            case "sin":
			PushElement(new Sine());
                break;
            case "cos":
			PushElement(new Cosine());
                break;
            case "tan":
			PushElement(new Tangent());
                break;
        }
    }

    public void OnInverseTrigonometricClick(string type) {
        switch (type) {
            case "sin":
			PushElement(new Arcsine());
                break;
            case "cos":
			PushElement(new Arccosine());
                break;
            case "tan":
			PushElement(new Arctangent());
                break;
        }
    }

    public void OnHyperbolicClick(string type) {
        switch (type) {
            case "sin":
			PushElement(new Sinh());
                break;
            case "cos":
			PushElement(new Cosh());
                break;
            case "tan":
			PushElement(new Tanh());
                break;
        }
    }

    public void OnInverseHyperbolicClick(string type) {
        switch (type) {
            case "sin":
			PushElement(new Arsinh());
                break;
            case "cos":
			PushElement(new Arcosh());
                break;
            case "tan":
			PushElement(new Artanh());
                break;
        }
    }

	public void OnRNDClick(){
		isDeg = !isDeg;
	}

    #endregion
}

