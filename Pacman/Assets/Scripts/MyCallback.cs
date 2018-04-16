using System;

using Callback = it.unical.mat.embasp.@base.ICallback;
using Output = it.unical.mat.embasp.@base.Output;
using AnswerSet = it.unical.mat.embasp.languages.asp.AnswerSet;
using AnswerSets = it.unical.mat.embasp.languages.asp.AnswerSets;
using System.IO;
using UnityEngine;
using Assets.Scripts.EmbASP.inputObjects;
using EmbASP3._5.it.unical.mat.embasp.languages.asp;

public class MyCallback : Callback
{
  public static int x = 0;
  private SymbolicConstant nextMove;
  public MyCallback(SymbolicConstant nextMove)
	{
    this.nextMove = nextMove;
	}

  public SymbolicConstant GetNextMove() {
    return nextMove;
  }

  public void SetNextMove(SymbolicConstant value) {
    nextMove = value;
  }

  void Callback.Callback(Output o)
	{
    Debug.Log("Output: " + MyCallback.x++ + " " + o.OutputString);
		AnswerSets answers = (AnswerSets) o;
    foreach (AnswerSet a in answers.Answersets)
		{

      try {
        foreach (object obj in a.Atoms) {
          //Debug.Log(obj.ToString());
          if (obj is Next) {
            Next nextAction = (Next)obj;
            SetNextMove(nextAction.getAction());
            Debug.Log("Next Action: " + GetNextMove());
          }
        }
      }
      catch (Exception e) {
        Console.WriteLine(e.ToString());
        Console.Write(e.StackTrace);
      }
		}
	}
  
}
