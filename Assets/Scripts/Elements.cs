using UnityEngine;
using System.Text;
using System.Collections;
using System;

public abstract class Element {

    public virtual string mark { get; protected set; }
}

public class ValueElement : Element {

    StringBuilder strValue = new StringBuilder();

    decimal? _value = null;
    public decimal value {
        get {
            if(_value == null) _value = decimal.Parse(strValue.ToString());
            return _value.Value;
        }
    }
    public override string mark {
        get {
            return strValue.ToString();
        }
        protected set {
            strValue = new StringBuilder(value);
        }
    }

    public ValueElement() {

    }

    public ValueElement(int baseValue) {
        strValue.Append(baseValue);
    }

    public ValueElement(decimal baseValue) {
        _value = baseValue;
    }

    public void Plus(int value) {
        if (strValue.ToString() == "0") {
            strValue = new System.Text.StringBuilder(value);
        }
        strValue.Append(value);
    }

    public void Dot() {
        string str = strValue.ToString();
        if (str[str.Length - 1] == '.') {
            strValue = new System.Text.StringBuilder(str.Remove(str.Length - 1));
        }
        else {
            if (!str.Contains(".")) {
                strValue.Append(".");
            }
        }
    }
}

public abstract class OperatorElement : Element {

    public int weight { get; protected set; }
    public int argCount { get; protected set; }

    public abstract ValueElement Calculate(params ValueElement[] values);
}

public class Add : OperatorElement {

    public Add() {
        weight = 4;
        argCount = 2;
        mark = "+";
    }

    public override ValueElement Calculate(params ValueElement[] values) {
        return new ValueElement(values[0].value + values[1].value);
    }
}

public class Subtract : OperatorElement {

    public Subtract() {
        weight = 4;
        argCount = 2;
        mark = "-";
    }

    public override ValueElement Calculate(params ValueElement[] values) {
        return new ValueElement(values[0].value - values[1].value);
    }
}

public class Multiply : OperatorElement {

    public Multiply() {
        weight = 3;
        argCount = 2;
        mark = "x";
    }

    public override ValueElement Calculate(params ValueElement[] values) {
        return new ValueElement(values[0].value * values[1].value);
    }
}

public class Divide : OperatorElement {

    public Divide() {
        weight = 3;
        argCount = 2;
        mark = "÷";
    }

    public override ValueElement Calculate(params ValueElement[] values) {
        return new ValueElement(values[0].value / values[1].value);
    }
}

public class Exponential : OperatorElement {

    public Exponential() {
        weight = 2;
        argCount = 2;
        mark = "^";
    }

    public override ValueElement Calculate(params ValueElement[] values) {
        return new ValueElement((decimal)Math.Pow((double)values[0].value , (double)values[1].value));
    }
}

public class Oparen : Element {
    public Oparen() {
        mark = "(";
    }
}

public class Cparen : Element {
    public Cparen() {
        mark = ")";
    }
}
