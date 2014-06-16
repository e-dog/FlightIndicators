using System;
using System.Text;
using System.Collections.Generic;


namespace Keramzit {


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
      else if (token("("))
      {
        var args=new List<Ast>(4);
        while (!token(")"))
        {
          if (atEnd()) return new Error(text, pos, sub, "unmatched ( in function call");

          pos=savePos();
          var a=expr();
          if (a==null) return new Error(text, pos, sub, "expected function argument");
          args.Add(a);

          if (token(",")) {}
          else if (token(")")) break;
          else return new Error(text, pos, sub, "expected , or )");
        }

        sub=new FuncCall(sub, args.ToArray());
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
