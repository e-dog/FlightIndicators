using System;
using System.Text;
using System.Collections.Generic;


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
    if (name=="orbit") return new ObjectWrapper(new WrapTest());
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


public class Parser
{
  string text;
  int curPos;


  Parser(string t)
  {
    text=t;
    curPos=0;
  }


  int savePos()
  {
    return curPos;
  }

  void restorePos(int p)
  {
    curPos=p;
  }


  bool atEnd() { return curPos>=text.Length; }


  void skipWS()
  {
    while (!atEnd() && char.IsWhiteSpace(text, curPos)) ++curPos;
  }


  bool ident(out string name)
  {
    if (atEnd()) { name=null; return false; }

    var c=text[curPos];
    if (c!='_' && !char.IsLetter(c)) { name=null; return false; }

    var str=new List<char>(32);
    str.Add(c);

    for (;;)
    {
      ++curPos;
      if (atEnd()) break;
      c=text[curPos];
      if (c!='_' && !char.IsLetterOrDigit(c)) break;
      str.Add(c);
    }

    name=new string(str.ToArray());
    return true;
  }


  bool token(string name)
  {
    var pos=savePos();
    skipWS();
    if (string.Compare(text, curPos,
      name, 0, name.Length, StringComparison.Ordinal)==0)
    {
      curPos+=name.Length;
      return true;
    }

    restorePos(pos);
    return false;
  }


  Ast expr()
  {
    return postOp();
  }


  Ast postOp()
  {
    var sub=primExpr();
    if (sub==null) return null;

    for (;;)
    {
      var pos=savePos();
      if (token("."))
      {
        string tag=null;
        if (!ident(out tag)) return new Error(text, pos, sub, "expected identifier after .");
        sub=new DotOp(sub, tag);
      }
      else
        { restorePos(pos); break; }
    }

    return sub;
  }


  Ast primExpr()
  {
    string id=null;
    if (ident(out id)) return new NameAtom(id);
    return null;
  }


  static public Ast parseExpr(string text)
  {
    if (string.IsNullOrEmpty(text)) return null;

    var p=new Parser(text);
    var e=p.expr();
    if (e==null || e is Error) return e;
    p.skipWS();
    if (p.atEnd()) return e;
    return new Error(text, p.curPos, e, "extra text after end of expression");
  }
}


//ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ//


} // namespace
