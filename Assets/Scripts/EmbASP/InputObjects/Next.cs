using EmbASP3._5.it.unical.mat.embasp.languages.asp;
using it.unical.mat.embasp.languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Assets.Scripts.EmbASP.inputObjects {
  [Id("next")]
  class Next {

    [Param(0)]
    private SymbolicConstant action;
    [Param(1)]
    private int time;

    public Next() { }

    public int getTime() {
      return time;
    }

    public void setTime(int value) {
      time = value;
    }

    public SymbolicConstant getAction() {
      return action;
    }

    public void setAction(SymbolicConstant value) {
      action = value;
    }

    public override string ToString() {
      return "next(" + action.Value + "," + time + ").";
    }
  }
}
