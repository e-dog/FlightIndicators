using System;
using System.Reflection;
using System.Collections.Generic;


namespace Keramzit {


//ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ//


public class WrapTest
{
  public double test=12345.6789;

  public int PeA { get; set; }

  public WrapTest() { PeA=654321; }
}


//ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ//


public class MethodWrapper
{
  protected object obj;
  protected MethodInfo method;

  public MethodWrapper(object o, MethodInfo m)
  {
    obj=o; method=m;
  }

  public object invoke(object[] args)
  {
    var r=method.Invoke(obj, args);
    return ObjectWrapper.wrap(r);
  }
}


//ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ//


public class ObjectWrapper
{
  protected object obj;
  protected Type type;

  public ObjectWrapper(object o) { obj=o; type=o.GetType(); }
  public ObjectWrapper(object o, Type t) { obj=o; type=t; }

  public object getMember(string name)
  {
    BindingFlags flags =
      BindingFlags.Public |
      BindingFlags.Instance |
      BindingFlags.Static |
      BindingFlags.GetField |
      BindingFlags.GetProperty;

    var members=type.GetMember(name, flags);
    if (members==null || members.Length!=1) return null;

    var prop=members[0] as PropertyInfo;
    if (prop!=null) return wrap(prop.GetValue(obj, null));

    var field=members[0] as FieldInfo;
    if (field!=null) return wrap(field.GetValue(obj));

    var method=members[0] as MethodInfo;
    if (method!=null) return new MethodWrapper(obj, method);

    return null;
  }

  public override string ToString()
  {
    if (obj==null) return "[null]";
    return obj.ToString();
  }

  public static object wrap(object o)
  {
    if (o==null) return null;

    if (o is ObjectWrapper || o is MethodWrapper
      || o is int || o is string || o is float || o is double || o is char) return o;

    return new ObjectWrapper(o);
  }
}


//ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ//


} // namespace
