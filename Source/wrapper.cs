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


public class ObjectWrapper
{
  protected object obj;

  public ObjectWrapper(object o) { obj=o; }

  public object getProp(string name)
  {
    if (obj==null) return null;

    BindingFlags flags =
      BindingFlags.Public |
      BindingFlags.Instance |
      BindingFlags.Static |
      BindingFlags.GetField |
      BindingFlags.GetProperty;
      // BindingFlags.FlattenHierarchy;

    var type=obj.GetType();

    // var prop=type.GetProperty(name, flags);
    // if (prop!=null) return prop.GetValue(obj, null);

    var members=type.GetMember(name, flags);
    if (members==null || members.Length!=1) return null;

    var prop=members[0] as PropertyInfo;
    if (prop!=null) return wrap(prop.GetValue(obj, null));

    var field=members[0] as FieldInfo;
    if (field!=null) return wrap(field.GetValue(obj));

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
    if (o is ObjectWrapper || o is int || o is string || o is float || o is double || o is char) return o;
    return new ObjectWrapper(o);
  }
}


//ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ//


} // namespace
