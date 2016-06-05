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

		const int MAX_LENGTH = 10;

		string stringValue;
		decimal realValue;
		public decimal value { get { return realValue; } }

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

		// 連結兩個數字
		public void Link(ValueElement e) {
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

		public void Backspace(){
			if (string.IsNullOrEmpty (stringValue))
				return;
			stringValue = stringValue.Remove (stringValue.Length - 1);
		}

		// 結束編輯
		public void FinishEdit() {
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
		public void Dot() {
			if (stringValue.Contains(".")) {
				if(stringValue.Last() == '.') {
					stringValue = stringValue.Remove(stringValue.Length - 1);
				}
			} else {
				stringValue += ".";
			}
		}

		// 正負號
		public void Sigh(){
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

		public abstract ValueElement Calculate(params ValueElement[] values);
	}

	// 加
	public class Add : OperatorElement {

		public Add() {
			weight = 4;
			argCount = 2;
		}

		public override ValueElement Calculate(params ValueElement[] values) {
			return new ValueElement(values[0].value + values[1].value);
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

		public override ValueElement Calculate(params ValueElement[] values) {
			return new ValueElement(values[0].value - values[1].value);
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

		public override ValueElement Calculate(params ValueElement[] values) {
			return new ValueElement(values[0].value * values[1].value);
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

		public override ValueElement Calculate(params ValueElement[] values) {
			return new ValueElement(values[0].value / values[1].value);
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

		public override ValueElement Calculate(params ValueElement[] values) {
			return new ValueElement((decimal)Math.Pow((double)values[0].value , (double)values[1].value));
		}

		public override string ToString() {
			return "^";
		}
	}

	// 平方
	public class Square : OperatorElement{
		public Square(){
			weight = 2;
			argCount = 1;
		}

		public override ValueElement Calculate(params ValueElement[] values){
			return new ValueElement (values [0].value * values [0].value);
		}

		public override string ToString ()
		{
			return string.Format ("²");
		}
	}

	// 開方
	public class Sqrt : OperatorElement{
		public Sqrt(){
			weight = 2;
			argCount = 1;
		}

		public override ValueElement Calculate(params ValueElement[] values){
			decimal result = (decimal)Math.Sqrt ((double)values [0].value);
			return new ValueElement (result);
		}

		public override string ToString ()
		{
			return "√ ";
		}
	}

	// 前括號
	public class Oparen : OperatorElement {

		public Oparen(){
			weight = int.MaxValue;
		}

		public override ValueElement Calculate (params ValueElement[] values)
		{
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

		public override ValueElement Calculate (params ValueElement[] values)
		{
			return null;
		}

		public override string ToString() {
			return ")";
		}
	}

	public class Error : Element{
		public override string ToString ()
		{
			return "[Error]";
		}
	}

	#region 進階函數

	public class Random : OperatorElement{
		public Random(){
			argCount = 0;
			weight = 1;
		}

		public override ValueElement Calculate (params ValueElement[] values)
		{
			return new ValueElement (UnityEngine.Random.value);
		}

		public override string ToString ()
		{
			return "[Rand]";
		}
	}

	public class MathematicalConstant : OperatorElement{
		public MathematicalConstant(){
			argCount = 0;
			weight = 1;
		}

		public override ValueElement Calculate (params ValueElement[] values)
		{
			return new ValueElement (Math.E);
		}

		public override string ToString ()
		{
			return "<i>e</i>";
		}
	}

	public class Factorial : OperatorElement{
		public Factorial(){
			argCount = 1;
			weight = 1;
		}

		public override ValueElement Calculate (params ValueElement[] values)
		{
			decimal factor = 1m;
			for (int i = 1; i <= values [0].value; i++) {
				factor *= i;
			}
			return new ValueElement (factor);
		}

		public override string ToString ()
		{
			return "!";
		}
	}

	public class Pi : OperatorElement{
		public Pi(){
			argCount = 0;
			weight = 1;
		}

		public override ValueElement Calculate (params ValueElement[] values)
		{
			return new ValueElement (Math.PI);
		}

		public override string ToString ()
		{
			return "π";
		}
	}

	public class LogE : OperatorElement{
		public LogE(){
			argCount = 1;
			weight = 1;
		}

		public override ValueElement Calculate (params ValueElement[] values)
		{
			return new ValueElement (Math.Log((double)values[0].value));
		}

		public override string ToString ()
		{
			return "ln";
		}
	}

	public class Log : OperatorElement{
		public Log(){
			argCount = 2;
			weight = 1;
		}

		public override ValueElement Calculate (params ValueElement[] values)
		{
			return new ValueElement (Math.Log((double)values[1].value, (double)values[0].value));
		}

		public override string ToString ()
		{
			return "log";
		}
	}

	public class Log10 : OperatorElement{
		public Log10(){
			argCount = 1;
			weight = 1;
		}

		public override ValueElement Calculate (params ValueElement[] values)
		{
			return new ValueElement (Math.Log10((double)values[0].value));
		}

		public override string ToString ()
		{
			return "log<size=15>10</size>";
		}
	}

	public class Log2 : OperatorElement{
		public Log2(){
			argCount = 1;
			weight = 1;
		}

		public override ValueElement Calculate (params ValueElement[] values)
		{
			return new ValueElement (Math.Log((double)values[0].value, 2));
		}

		public override string ToString ()
		{
			return "log<size=15>2</size>";
		}
	}

	public class Sine : OperatorElement{
		public Sine(){
			argCount = 1;
			weight = 1;
		}

		public override ValueElement Calculate (params ValueElement[] values)
		{
			return new ValueElement (Math.Sin((double)values[0].value));
		}

		public override string ToString ()
		{
			return "sin";
		}
	}

	public class Arcsine : OperatorElement{
		public Arcsine(){
			argCount = 1;
			weight = 1;
		}

		public override ValueElement Calculate (params ValueElement[] values)
		{
			return new ValueElement (Math.Asin ((double)values [0].value));
		}

		public override string ToString ()
		{
			return "arcsin";
		}
	}

	public class Sinh : OperatorElement{
		public Sinh(){
			argCount = 1;
			weight = 1;
		}

		public override ValueElement Calculate (params ValueElement[] values)
		{
			return new ValueElement (Math.Sinh ((double)values [0].value));
		}

		public override string ToString ()
		{
			return "sinh";
		}
	}

	public class Cosine : OperatorElement{
		public Cosine(){
			argCount = 1;
			weight = 1;
		}

		public override ValueElement Calculate (params ValueElement[] values)
		{
			return new ValueElement (Math.Cos((double)values[0].value));
		}

		public override string ToString ()
		{
			return "cos";
		}
	}

	public class Arccosine : OperatorElement{
		public Arccosine(){
			argCount = 1;
			weight = 1;
		}

		public override ValueElement Calculate (params ValueElement[] values)
		{
			return new ValueElement (Math.Acos ((double)values [0].value));
		}

		public override string ToString ()
		{
			return "arccos";
		}
	}

	public class Cosh : OperatorElement{
		public Cosh(){
			argCount = 1;
			weight = 1;
		}

		public override ValueElement Calculate (params ValueElement[] values)
		{
			return new ValueElement (Math.Cosh ((double)values [0].value));
		}

		public override string ToString ()
		{
			return "cosh";
		}
	}

	public class Tangent : OperatorElement{
		public Tangent(){
			argCount = 1;
			weight = 1;
		}

		public override ValueElement Calculate (params ValueElement[] values)
		{
			return new ValueElement (Math.Tan((double)values[0].value));
		}

		public override string ToString ()
		{
			return "tan";
		}
	}

	public class Arctangent : OperatorElement{
		public Arctangent(){
			argCount = 1;
			weight = 1;
		}

		public override ValueElement Calculate (params ValueElement[] values)
		{
			return new ValueElement (Math.Atan ((double)values [0].value));
		}

		public override string ToString ()
		{
			return "arctan";
		}
	}

	public class Tanh : OperatorElement{
		public Tanh(){
			argCount = 1;
			weight = 1;
		}

		public override ValueElement Calculate (params ValueElement[] values)
		{
			return new ValueElement (Math.Tanh ((double)values [0].value));
		}

		public override string ToString ()
		{
			return "tanh";
		}
	}

	#endregion
}