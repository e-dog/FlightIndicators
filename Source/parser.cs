using System;
using System.Text;
using System.Collections.Generic;


namespace Keramzit {


//ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ//


public class Ast
{
}


public class NameAtom : Ast
{
  public string name;

  public NameAtom(string n) { name=n; }
  public override string ToString() { return "NameAtom("+name+")"; }
}


public class UnaryOp : Ast
{
  public Ast sub;

  public UnaryOp(Ast s) { sub=s; }
}


public class DotOp : UnaryOp
{
  public string tag;

  public DotOp(Ast s, string t) : base(s) { tag=t; }
  public override string ToString() { return "DotOp("+sub.ToString()+", "+tag+")"; }
}


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
      string tag=null;
      if (token(".") && ident(out tag))
        sub=new DotOp(sub, tag);
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
    return p.expr();
  }
}


//ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ//


} // namespace
