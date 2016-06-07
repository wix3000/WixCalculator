using UnityEngine;
using System.Text;
using System.Collections;
using System;
using System.Linq;

namespace Wix.Calculator{

	public abstract class Element {
		public override abstract string ToString();
	}

	public class ValueElement : Element {

        protected const int MAX_LENGTH = 10;

		protected string stringValue;
        protected decimal realValue;
		public decimal value { get { return realValue; } }

        public ValueElement() {}

		public ValueElement(int i) {
			realValue = i;
			stringValue = i.ToString();
		}

		public ValueElement(double value) {
			realValue = (decimal)value;
			stringValue = value.ToString();
		}

		public ValueElement(decimal value) {
			realValue = value;
			stringValue = value.ToString();
		}

        public ValueElement(string value) {
            if (value.Length > MAX_LENGTH) value = value.Substring(0, MAX_LENGTH);
            stringValue = value;
            realValue = decimal.Parse(value);
        }

		// 連結兩個數字
		public virtual void Link(ValueElement e) {
			if (stringValue.Length > MAX_LENGTH)
				return;

			if (stringValue == "0") {
				stringValue = e.stringValue;
				realValue = e.realValue;
			} else {
				stringValue += e.stringValue;
				realValue = decimal.Parse (stringValue);
			}
		}

        // 倒退
		public virtual void Backspace(){
			if (string.IsNullOrEmpty (stringValue))
				return;
			stringValue = stringValue.Remove (stringValue.Length - 1);
		}

		// 結束編輯
		public virtual void FinishEdit() {
			realValue = decimal.Parse(stringValue);
			stringValue = realValue.ToString ();
		}

		// 轉為字串
		public override string ToString() {
			return stringValue;
		}

		public string ToString(string format) {
			return realValue.ToString(format);
		}

		// 加小數點
		public virtual void Dot() {
            if (stringValue.Contains(".")) {
				if(stringValue.Last() == '.') {
					stringValue = stringValue.Remove(stringValue.Length - 1);
				}
			} else {
				stringValue += ".";
			}
		}

		// 正負號
		public virtual void Sigh(){
			if (stringValue [0] == '-') {
				stringValue = stringValue.Remove (0, 1);
			} else {
				stringValue = $"-{stringValue}";
			}
		}
	}

	public abstract class OperatorElement : Element {

		public int weight { get; protected set; }
		public int argCount { get; protected set; }

		public abstract ValueElement Calculate(ValueElement right, ValueElement left);

        internal ValueElement Calculate(Element element1, Element element2) {
            return Calculate(element1 as ValueElement, element2 as ValueElement);
        }
    }

	public interface IFunction{
		bool isDiargument{ get; }
	}

    // 加
    public class Add : OperatorElement {

		public Add() {
			weight = 4;
			argCount = 2;
		}

		public override ValueElement Calculate(ValueElement right, ValueElement left) {
			return new ValueElement(left.value + right.value);
		}

		public override string ToString() {
			return "+";
		}
	}

	// 減
	public class Subtract : OperatorElement {

		public Subtract() {
			weight = 4;
			argCount = 2;
		}

		public override ValueElement Calculate(ValueElement right, ValueElement left) {
			return new ValueElement(left.value - right.value);
		}

		public override string ToString() {
			return "-";
		}
	}

	// 乘
	public class Multiply : OperatorElement {

		public Multiply() {
			weight = 3;
			argCount = 2;
		}

		public override ValueElement Calculate(ValueElement right, ValueElement left) {
			return new ValueElement(left.value * right.value);
		}

		public override string ToString() {
			return "x";
		}
	}

	// 除
	public class Divide : OperatorElement {

		public Divide() {
			weight = 3;
			argCount = 2;
		}

		public override ValueElement Calculate(ValueElement right, ValueElement left) {
			return new ValueElement(left.value / right.value);
		}

		public override string ToString() {
			return "/";
		}
	}

	// 除
	public class Divide2 : OperatorElement {

		public Divide2() {
			weight = 3;
			argCount = 2;
		}

		public override ValueElement Calculate(ValueElement right, ValueElement left) {
			return new ValueElement(left.value / right.value);
		}

		public override string ToString() {
			return "÷";
		}
	}

	// 次方
	public class Exponential : OperatorElement {

		public Exponential() {
			weight = 2;
			argCount = 2;
		}

		public override ValueElement Calculate(ValueElement right, ValueElement left) {
			return new ValueElement((decimal)Math.Pow((double)left.value , (double)right.value));
		}

		public override string ToString() {
			return "^";
		}
	}

	// 平方
	public class Square : OperatorElement, IFunction{

		public bool isDiargument { get; } = false;

        public Square(){
			weight = 2;
			argCount = 1;
		}

        public override ValueElement Calculate(ValueElement right, ValueElement left) {
            if (!string.IsNullOrEmpty(left.ToString())) {
                Debug.LogWarning($"{GetType().ToString()} Calculate Error");
                throw new Exception();
            }
			return new ValueElement (right.value * right.value);
		}

		public override string ToString ()
		{
			return string.Format ("x²");
		}
	}

	// 開方
	public class Sqrt : OperatorElement, IFunction{

		public bool isDiargument { get; } = false;

        public Sqrt(){
			weight = 2;
			argCount = 1;
		}

		public override ValueElement Calculate(ValueElement right, ValueElement left) {
            if (!string.IsNullOrEmpty(left.ToString())) {
                Debug.LogWarning($"{GetType().ToString()} Calculate Error");
                throw new Exception();
            }

            decimal result = (decimal)Math.Sqrt ((double)right.value);
			return new ValueElement (result);
		}

		public override string ToString ()
		{
			return "√x";
		}
	}

	// 前括號
	public class Oparen : OperatorElement {

		public Oparen(){
			weight = int.MaxValue;
		}

		public override ValueElement Calculate(ValueElement right, ValueElement left) {
			return null;
		}

		public override string ToString() {
			return "(";
		}
	}

	// 後括號
	public class Cparen : OperatorElement {

		public Cparen(){
			weight = int.MaxValue;
		}

		public override ValueElement Calculate(ValueElement right, ValueElement left) {
			return null;
		}

		public override string ToString() {
			return ")";
		}
	}

	public class Comma : OperatorElement{

		public Comma(){
			weight = int.MaxValue;
		}

		public override ValueElement Calculate (ValueElement right, ValueElement left)
		{
			throw new NotImplementedException ();
		}
			
		public override string ToString ()
		{
			return ", ";
		}
	}

	public class Error : Element{
		public override string ToString ()
		{
			return "[Error]";
		}
	}

	#region 進階函數

	// 代數
    public class Algebra : ValueElement {

        public Algebra(AlgebraType type) {
            switch (type) {
                case AlgebraType.E:
                    realValue = (decimal)Math.E;
                    stringValue = "<i>e</i>";
                    break;
                case AlgebraType.Pi:
                    realValue = (decimal)Math.PI;
                    stringValue = "π";
                    break;
            }
        }

        public override void Backspace() {
            if (string.IsNullOrEmpty(stringValue)) return;
            stringValue = string.Empty;
        }

        public override void Dot() {
            return;
        }

        public override void FinishEdit() {
            return;
        }

        public override void Link(ValueElement e) {
            return;
        }

        public override void Sigh() {
            realValue *= -1;
            stringValue = (realValue < 0) ? "-" + stringValue : stringValue.Replace("-", "");
        }

    }

	// 代數類型
    public enum AlgebraType {
        E,
        Pi
    }

	public class Factorial : OperatorElement, IFunction{
		public bool isDiargument { get; } = false;

        public Factorial(){
			argCount = 1;
			weight = 1;
		}

        public override ValueElement Calculate(ValueElement right, ValueElement left) {
			if (!string.IsNullOrEmpty(left.ToString())) {
                Debug.LogWarning($"{GetType().ToString()} Calculate Error");
                throw new Exception();
            }

            decimal factor = 1m;
			for (int i = 1; i <= right.value; i++) {
				factor *= i;
			}
			return new ValueElement (factor);
		}

		public override string ToString ()
		{
			return "x!";
		}
	}

	// 根號
	public class Root : OperatorElement, IFunction{

		public bool isDiargument { get; } = true;

		public Root(){
			weight = 5;
			argCount = 2;
		}

		public override ValueElement Calculate(ValueElement right, ValueElement left) {
			
			decimal result = (decimal) Math.Pow ((double)right.value, 1d / (double)left.value);
				
			return new ValueElement (result);
		}

		public override string ToString ()
		{
			return "y√x";
		}
	}

	public class LogE : OperatorElement, IFunction{
		public bool isDiargument { get; } = false;

        public LogE(){
			argCount = 1;
			weight = 1;
		}

		public override ValueElement Calculate(ValueElement right, ValueElement left) {
            if (!string.IsNullOrEmpty(left.ToString())) {
                Debug.LogWarning($"{GetType().ToString()} Calculate Error");
                throw new Exception();
            }

            return new ValueElement (Math.Log((double)right.value));
		}

		public override string ToString ()
		{
			return "ln";
		}
	}

	public class Log : OperatorElement, IFunction{
		public bool isDiargument { get; } = true;

		public Log(){
			argCount = 2;
			weight = 5;
		}

		public override ValueElement Calculate(ValueElement right, ValueElement left) {
            return new ValueElement(Math.Log((double)right.value, (double)left.value));
		}

		public override string ToString ()
		{
			return "log";
		}
	}

	public class Log10 : OperatorElement, IFunction{
		public bool isDiargument { get; } = false;

        public Log10(){
			argCount = 1;
			weight = 1;
		}

		public override ValueElement Calculate(ValueElement right, ValueElement left) {
            if (!string.IsNullOrEmpty(left.ToString())) {
                Debug.LogWarning($"{GetType().ToString()} Calculate Error");
                throw new Exception();
            }

            return new ValueElement (Math.Log10((double)right.value));
		}

		public override string ToString ()
		{
			return "log<size=15>10</size>";
		}
	}

	public class Log2 : OperatorElement, IFunction{
		public bool isDiargument { get; } = false;

        public Log2(){
			argCount = 1;
			weight = 1;
		}

		public override ValueElement Calculate(ValueElement right, ValueElement left) {
            if (!string.IsNullOrEmpty(left.ToString())) {
                Debug.LogWarning($"{GetType().ToString()} Calculate Error");
                throw new Exception();
            }

            return new ValueElement (Math.Log((double)right.value, 2));
		}

		public override string ToString ()
		{
			return "log<size=15>2</size>";
		}
	}

	public class Sine : OperatorElement, IFunction{
		public bool isDiargument { get; } = false;

        public Sine(){
			argCount = 1;
			weight = 1;
		}

		public override ValueElement Calculate(ValueElement right, ValueElement left) {
            if (!string.IsNullOrEmpty(left.ToString())) {
                Debug.LogWarning($"{GetType().ToString()} Calculate Error");
                throw new Exception();
            }
			double value = (double)right.value;
			if (Calculator.isDeg)
				value *= Mathf.Rad2Deg;

			return new ValueElement (Math.Sin(value));
		}

		public override string ToString ()
		{
			return "sin";
		}
	}

	public class Arcsine : OperatorElement, IFunction{
		public bool isDiargument { get; } = false;

        public Arcsine(){
			argCount = 1;
			weight = 1;
		}

		public override ValueElement Calculate(ValueElement right, ValueElement left) {
            if (!string.IsNullOrEmpty(left.ToString())) {
                Debug.LogWarning($"{GetType().ToString()} Calculate Error");
                throw new Exception();
            }
			double value = (double)right.value;
			if (Calculator.isDeg)
				value *= Mathf.Rad2Deg;

            return new ValueElement (Math.Asin ((double)right.value));
		}

		public override string ToString ()
		{
			return "arcsin";
		}
	}

	public class Sinh : OperatorElement, IFunction{
		public bool isDiargument { get; } = false;

        public Sinh(){
			argCount = 1;
			weight = 1;
		}

		public override ValueElement Calculate(ValueElement right, ValueElement left) {
            if (!string.IsNullOrEmpty(left.ToString())) {
                Debug.LogWarning($"{GetType().ToString()} Calculate Error");
                throw new Exception();
            }
			double value = (double)right.value;
			if (Calculator.isDeg)
				value *= Mathf.Rad2Deg;

            return new ValueElement (Math.Sinh ((double)right.value));
		}

		public override string ToString ()
		{
			return "sinh";
		}
	}

	public class Cosine : OperatorElement, IFunction{
		public bool isDiargument { get; } = false;

        public Cosine(){
			argCount = 1;
			weight = 1;
		}

		public override ValueElement Calculate(ValueElement right, ValueElement left) {
            if (!string.IsNullOrEmpty(left.ToString())) {
                Debug.LogWarning($"{GetType().ToString()} Calculate Error");
                throw new Exception();
            }
			double value = (double)right.value;
			if (Calculator.isDeg)
				value *= Mathf.Rad2Deg;

            return new ValueElement (Math.Cos((double)right.value));
		}

		public override string ToString ()
		{
			return "cos";
		}
	}

	public class Arccosine : OperatorElement, IFunction{
		public bool isDiargument { get; } = false;

        public Arccosine(){
			argCount = 1;
			weight = 1;
		}

		public override ValueElement Calculate(ValueElement right, ValueElement left) {
            if (!string.IsNullOrEmpty(left.ToString())) {
                Debug.LogWarning($"{GetType().ToString()} Calculate Error");
                throw new Exception();
            }
			double value = (double)right.value;
			if (Calculator.isDeg)
				value *= Mathf.Rad2Deg;

            return new ValueElement (Math.Acos ((double)right.value));
		}

		public override string ToString ()
		{
			return "arccos";
		}
	}

	public class Cosh : OperatorElement, IFunction{
		public bool isDiargument { get; } = false;

        public Cosh(){
			argCount = 1;
			weight = 1;
		}

		public override ValueElement Calculate(ValueElement right, ValueElement left) {
            if (!string.IsNullOrEmpty(left.ToString())) {
                Debug.LogWarning($"{GetType().ToString()} Calculate Error");
                throw new Exception();
            }
			double value = (double)right.value;
			if (Calculator.isDeg)
				value *= Mathf.Rad2Deg;

            return new ValueElement (Math.Cosh ((double)right.value));
		}

		public override string ToString ()
		{
			return "cosh";
		}
	}

	public class Tangent : OperatorElement, IFunction{
		public bool isDiargument { get; } = false;

        public Tangent(){
			argCount = 1;
			weight = 1;
		}

		public override ValueElement Calculate(ValueElement right, ValueElement left) {
            if (!string.IsNullOrEmpty(left.ToString())) {
                Debug.LogWarning($"{GetType().ToString()} Calculate Error");
                throw new Exception();
            }
			double value = (double)right.value;
			if (Calculator.isDeg)
				value *= Mathf.Rad2Deg;

            return new ValueElement (Math.Tan((double)left.value));
		}

		public override string ToString ()
		{
			return "tan";
		}
	}

	public class Arctangent : OperatorElement, IFunction{
		public bool isDiargument { get; } = false;

        public Arctangent(){
			argCount = 1;
			weight = 1;
		}

		public override ValueElement Calculate(ValueElement right, ValueElement left) {
            if (!string.IsNullOrEmpty(left.ToString())) {
                Debug.LogWarning($"{GetType().ToString()} Calculate Error");
                throw new Exception();
            }
			double value = (double)right.value;
			if (Calculator.isDeg)
				value *= Mathf.Rad2Deg;

            return new ValueElement (Math.Atan ((double)right.value));
		}

		public override string ToString ()
		{
			return "arctan";
		}
	}

	public class Tanh : OperatorElement, IFunction{
		public bool isDiargument { get; } = false;

        public Tanh(){
			argCount = 1;
			weight = 1;
		}

		public override ValueElement Calculate(ValueElement right, ValueElement left) {
            if (!string.IsNullOrEmpty(left.ToString())) {
                Debug.LogWarning($"{GetType().ToString()} Calculate Error");
                throw new Exception();
            }
			double value = (double)right.value;
			if (Calculator.isDeg)
				value *= Mathf.Rad2Deg;

            return new ValueElement (Math.Tanh ((double)right.value));
		}

		public override string ToString ()
		{
			return "tanh";
		}
	}

	public class Arsinh : OperatorElement, IFunction{
		public bool isDiargument { get; } = false;

        public Arsinh() {
            weight = 1;
            argCount = 1;
        }

        public override ValueElement Calculate(ValueElement right, ValueElement left) {
            if (!string.IsNullOrEmpty(left.ToString())) {
                Debug.LogWarning($"{GetType().ToString()} Calculate Error");
                throw new Exception();
            }
            double x = (double)right.value;
			if (Calculator.isDeg)
				x *= Mathf.Rad2Deg;

            double result = Math.Log(x + Math.Sqrt(Math.Pow(x, 2d) + 1d));
            return new ValueElement(result);
        }

        public override string ToString() {
            return "arsinh";
        }
    }

	public class Arcosh : OperatorElement, IFunction{
		public bool isDiargument { get; } = false;

        public Arcosh() {
            weight = 1;
            argCount = 1;
        }

        public override ValueElement Calculate(ValueElement right, ValueElement left) {
            if (!string.IsNullOrEmpty(left.ToString())) {
                Debug.LogWarning($"{GetType().ToString()} Calculate Error");
                throw new Exception();
            }

            double x = (double)right.value;
			if (Calculator.isDeg)
				x *= Mathf.Rad2Deg;
			
            double result = Math.Log(x + Math.Sqrt(Math.Pow(x, 2d) - 1d));
            return new ValueElement(result);
        }

        public override string ToString() {
            return "arcosh";
        }
    }

	public class Artanh : OperatorElement, IFunction{
		public bool isDiargument { get; } = false;

        public Artanh() {
            weight = 1;
            argCount = 1;
        }

        public override ValueElement Calculate(ValueElement right, ValueElement left) {
            if (!string.IsNullOrEmpty(left.ToString())) {
                Debug.LogWarning($"{GetType().ToString()} Calculate Error");
                throw new Exception();
            }
            double x = (double)right.value;
			if (Calculator.isDeg)
				x *= Mathf.Rad2Deg;
			
            double result = 0.5d * Math.Log((1 + x) / (1 - x));
            return new ValueElement(result);
        }

        public override string ToString() {
            return "artanh";
        }
    }

    #endregion
}