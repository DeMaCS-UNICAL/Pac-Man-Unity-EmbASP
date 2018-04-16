using it.unical.mat.embasp.languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts {

  [Id("a")]
  class A {

    [Param(0)]
    private int x;

    public A() {
    }

    public int getX() { return x; }
    public void setX(int x) { this.x = x; }

    public override string ToString() {
      return "a(" + x + ").";
    }

  }
}
