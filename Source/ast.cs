using System;
using System.Text;


namespace Keramzit {


//ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ//


public abstract class Ast
{
  public abstract object eval();
}


public class Error : Ast
{
  public string text;
  public int pos;
  public Ast parsed;
  public string msg;

  public Error(string t, int p, Ast a, string m) { text=t; pos=p; parsed=a; msg=m; }
  public override string ToString()
  {
    return "Error at '"+text.Substring(pos)+"' (offset "+pos+"): "+msg;
  }

  public override object eval()
  {
    return null;
  }
}


public class NameAtom : Ast
{
  public string name;

  public NameAtom(string n) { name=n; }
  public override string ToString() { return "NameAtom("+name+")"; }

  public override object eval()
  {
    if (name=="orbit") return new ObjectWrapper(FlightGlobals.ship_orbit);
    return null;
  }
}


public abstract class UnaryOp : Ast
{
  public Ast sub;

  public UnaryOp(Ast s) { sub=s; }
}


public class DotOp : UnaryOp
{
  public string tag;

  public DotOp(Ast s, string t) : base(s) { tag=t; }
  public override string ToString() { return "DotOp("+sub.ToString()+", "+tag+")"; }

  public override object eval()
  {
    var v=sub.eval();
    if (v==null) return null;

    var ow=v as ObjectWrapper;
    if (ow!=null) return ow.getProp(tag);

    return null;
  }
}


//ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ//


} // namespace
