using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Wix.Calculator{
	public class Calculator {

		System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch ();

		public List<Element> equation { get; private set; }
		public TimeSpan spendTime{ get { return timer.Elapsed; } }
		public Element result { get; private set; }
		public static bool isDeg;

		public Calculator(){
		}

		public Calculator(List<Element> equation, bool isDeg = false){
			Calculator.isDeg = isDeg;
			Execute (equation);
		}

		public Element Execute(List<Element> equation){
			// 複製一份計算式
			this.equation = new List<Element> (equation);

			// 計時
			timer.Reset();
			timer.Start();
			CalculateFlowchart ();
			timer.Stop ();
			Debug.Log ($"Spend Time: {spendTime.ToString()}");
			return result;
		}

		void CalculateFlowchart(){
			try{
				List<Element> es = new List<Element>(equation);
				PreFix(es);
				es = InfixToPostfix(es);
				Evaluate(es);
			}
			catch{
				timer.Stop ();
				result = new Error ();
			}
		}

		/// <summary>
		/// 對運算式預處理
		/// </summary>
		/// <param name="elements">Elements.</param>
		public void PreFix(List<Element> elements){
            AddMultiply(elements);
            AddNullElement(elements);
		}

        void AddMultiply(List<Element> elements) {
            for (int i = 0; i < elements.Count; i++) {
                Element e = elements[i];

                if (e is Cparen && i != elements.Count - 1) {
                    e = elements[i + 1];
                    if (e is Oparen || e is ValueElement)
                        elements.Insert(i + 1, new Multiply());
                }
                else if (e is Oparen && i != 0) {
                    e = elements[i - 1];
                    if (e is Cparen || e is ValueElement)
                        elements.Insert(i, new Multiply());
                }
            }
        }

        void AddNullElement(List<Element> elements) {
            for(int i = 0; i < elements.Count; i++) {
				IFunction e = elements[i] as IFunction;
                if (e == null) continue;

				if (!e.isDiargument) {
					elements.Insert (i, new ValueElement ());
					i++;
				}
            }
        }

		/// <summary>
		/// 將中序表達式轉換為後序表達式
		/// </summary>
		/// <returns>The to postfix.</returns>
		/// <param name="elements">Elements.</param>
		public List<Element> InfixToPostfix(List<Element> elements){
			List<Element> postfix = new List<Element> ();
			Stack<Element> stack = new Stack<Element> ();
			Stack<Element> diarg = new Stack<Element> ();

			for (int i = 0; i < elements.Count; i++) {
				Element e = elements [i];

				if (e is Oparen) {
					stack.Push (e);
				} else if (e is Cparen) {
					ProcessCparen (stack, postfix);
				} else if (e is Comma) {
					ProcessOperatorStack (diarg.Pop (), stack, postfix);
				} else if (e is IFunction && ((IFunction)e).isDiargument) {
					diarg.Push (e);
				} else if (e is OperatorElement) {
					ProcessOperatorStack (e, stack, postfix);
				} else {
					postfix.Add (e);
				}
			}

			while (stack.Count > 0) {
				postfix.Add (stack.Pop ());
			}

			return postfix;
		}

		void ProcessOperatorStack(Element e, Stack<Element> stack, List<Element> postfix){
			while (stack.Count > 0) {
				Element top = stack.Peek ();
				if (((OperatorElement)e).weight < ((OperatorElement)top).weight) {
					break;
				} else {
					postfix.Add (stack.Pop ());
				}
			}
			stack.Push (e);
		}

		void ProcessCparen(Stack<Element> stack, List<Element> postfix){
			while (stack.Count > 0) {
				Element top = stack.Pop ();
				if (top is Oparen) {
					return;
				} else {
					postfix.Add (top);
				}
			}
			Debug.LogError("括號未成對");
			throw new Exception ("括號未成對");
		}

		/// <summary>
		/// 為後序表達式求值
		/// </summary>
		/// <param name="postfix">Postfix.</param>
		public void Evaluate(List<Element> postfix){
			Stack<Element> stack = new Stack<Element> ();

			foreach (Element e in postfix) {
				if (e is ValueElement) {
					stack.Push (e);
				} else if (e is OperatorElement) {
                    //int argCount = ((OperatorElement)e).argCount;
                    //ValueElement[] args = new ValueElement[argCount];
                    //for (int j = argCount - 1; j >= 0; j--) {
                    //	args [j] = (ValueElement)stack.Pop ();
                    //}
                    //ValueElement tempResult = ((OperatorElement)e).Calculate (args);
                    ValueElement tempResult = ((OperatorElement)e).Calculate(stack.Pop(), stack.Pop());
					stack.Push (tempResult);
				}
			}

			//Check
			if (stack.Count != 1) {
				throw new Exception ("運算錯誤");
			} else {
				result = stack.Pop ();
			}
		}
	}
}
