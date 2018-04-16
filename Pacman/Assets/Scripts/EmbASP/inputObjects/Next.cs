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

    public Next() { }

    public SymbolicConstant getAction() {
      return action;
    }

    public void setAction(SymbolicConstant value) {
      action = value;
    }
  }
}
