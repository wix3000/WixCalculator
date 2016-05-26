using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class Calculator : MonoBehaviour {

    public static List<Element> elements = new List<Element> { new ValueElement(0) };

    [SerializeField]
    Text inputLine;

    void Start() {

    }

    void RefreshInputLine() {
        StringBuilder sb = new StringBuilder();
        for(int i = 0; i < elements.Count; i++) {
            sb.Append(elements[i].mark);
            print(elements[i].mark);
            if(i != elements.Count - 1) {
                sb.Append(" ");
            }
        }
        inputLine.text = sb.ToString();
    }

    public void EnterValue(int v) {
        if (elements[elements.Count - 1] is ValueElement) {
            (elements[elements.Count - 1] as ValueElement).Plus(v);
        }
        else {
            elements.Add(new ValueElement(v));
        }
        RefreshInputLine();
    }

    public void Dot() {
        if (elements[elements.Count - 1] is ValueElement) {
            (elements[elements.Count - 1] as ValueElement).Dot();
        }
        RefreshInputLine();
    }

    public void PlusBaseOperator(string op) {
        if (elements[elements.Count - 1] is ValueElement ||
            elements[elements.Count - 1] is Cparen) {
            switch (op) {
                case "Add":
                    elements.Add(new Add());
                    break;
                case "Divide":
                    elements.Add(new Divide());
                    break;
                case "Multiply":
                    elements.Add(new Multiply());
                    break;
                case "Subtract":
                    elements.Add(new Subtract());
                    break;
                case "Exponential":
                    elements.Add(new Exponential());
                    break;
            }
        }
    }
}