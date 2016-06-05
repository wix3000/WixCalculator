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
		inputLine.text = (element.ToString ().Length > 10) ? ((ValueElement)element).ToString ("E") : element.ToString ();
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

    #region 基本按鍵區
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

    #endregion

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
        print(UnityEngine.Random.value.ToString());
        PushAlgebra(new ValueElement(UnityEngine.Random.value.ToString()));
	}

	public void OnEClick(){
        PushAlgebra(new Algebra(AlgebraType.E));
	}

    public void OnPiClick() {
        PushAlgebra(new Algebra(AlgebraType.Pi));
    }

	public void OnExponentialClick(){
		PushOperator (new Exponential ());
	}

	public void OnFactorialClick(){
		PushOperator (new Factorial ());
	}

    public void OnLogClick(string baseValue) {
        switch (baseValue) {
            case "10":
                PushOperator(new Log10());
                break;
            case "2":
                PushOperator(new Log2());
                break;
            case "e":
                PushOperator(new LogE());
                break;
            default:
                PushOperator(new Log());
                break;
        }
    }

    public void OnTrigonometricClick(string type) {
        switch (type) {
            case "sin":
                PushOperator(new Sine());
                break;
            case "cos":
                PushOperator(new Cosine());
                break;
            case "tan":
                PushOperator(new Tangent());
                break;
        }
    }

    public void OnInverseTrigonometricClick(string type) {
        switch (type) {
            case "sin":
                PushOperator(new Arcsine());
                break;
            case "cos":
                PushOperator(new Arccosine());
                break;
            case "tan":
                PushOperator(new Arctangent());
                break;
        }
    }

    public void OnHyperbolicClick(string type) {
        switch (type) {
            case "sin":
                PushOperator(new Sinh());
                break;
            case "cos":
                PushOperator(new Cosh());
                break;
            case "tan":
                PushOperator(new Tanh());
                break;
        }
    }

    public void OnInverseHyperbolicClick(string type) {
        switch (type) {
            case "sin":
                PushOperator(new Arsinh());
                break;
            case "cos":
                PushOperator(new Arcosh());
                break;
            case "tan":
                PushOperator(new Artanh());
                break;
        }
    }

    #endregion
}

