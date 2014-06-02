using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using KSP;


namespace Keramzit {


//ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ//


[KSPAddon(KSPAddon.Startup.Flight, false)]
public class FlightIndicatorsGUI : MonoBehaviour
{
  ScreenSafeGUIText myText;


  public void Awake()
  {
  }


  public static string DumpObjectFields(object obj)
  {
      BindingFlags flags = BindingFlags.Default;

      StringBuilder sb = new StringBuilder();
      Type type = obj.GetType();

      foreach (FieldInfo field in type.GetFields(flags))
      {
          object value = field.GetValue(obj);
          if (value == null)
              sb.AppendLine(field.FieldType.Name + " " + field.Name + "is null");
          else
              sb.AppendLine(field.FieldType.Name + " " + field.Name + " = " + value);
      }
      return sb.ToString();
  }


  public static string DumpObjectProps(object obj)
  {
      BindingFlags flags = BindingFlags.Default;

      StringBuilder sb = new StringBuilder();
      Type type = obj.GetType();

      foreach (PropertyInfo prop in type.GetProperties(flags))
      {
          object value = prop.GetValue(obj, null);
          if (value == null)
              sb.AppendLine(prop.PropertyType.Name + " " + prop.Name + "is null");
          else
              sb.AppendLine(prop.PropertyType.Name + " " + prop.Name + " = " + value);
      }
      return sb.ToString();
  }


  void dumpGuiObj(Transform t)
  {
    print("obj "+t.gameObject.name);
    if (t.parent!=null) print("  parent "+t.parent.gameObject.name);

    foreach (var c in t.GetComponents<Component>())
    {
      print("  comp "+c.GetType().Name);
    }

    if (t.renderer) print("tex: "+t.renderer.material.mainTexture+" shader: "+t.renderer.material.shader.name);

    foreach (Transform c in t)
      dumpGuiObj(c);
  }


  public void onDestroy()
  {
    if (myText!=null)
    {
      myText.transform.parent=null;
      UnityEngine.Object.Destroy(myText.gameObject);
      myText=null;
    }
  }


  public void LateUpdate()
  {
    var fmc=FlightUIModeController.Instance;
    if (fmc!=null)
    {
      var navBall=fmc.navBall;
      if (navBall!=null && myText==null)
      {
        var t=FlightUIController.fetch.speed;
        var o=new GameObject("KzFlightIndicators");
        o.transform.parent=navBall.transform;
        o.transform.localPosition=new Vector3(-0.38f, 0.23f, -0.5f);
        // o.transform.localRotation=Quaternion.identity;

        myText=o.AddComponent<ScreenSafeGUIText>();
        myText.text="";
        for (int i=0; i<12; ++i) myText.text+="Periapsis 987654 Gm\n";
        myText.textSize=12;
        myText.textStyle=new GUIStyle(t.textStyle);
        myText.textStyle.alignment=TextAnchor.UpperLeft;
        //==

        // shader: "Unlit/Transparent Tint"

        // navBall frame relative to navBall:
        // v 0: [-0.139279067516327, 0.224930435419083, -0.55064731836319]  [0, 1, 0]
        // v 1: [0.142255127429962, 0.224930435419083, -0.550647377967834]  [1, 1, 0]
        // v 2: [-0.139279067516327, -0.0136603936553001, -0.550647377967834]  [0, 0, 0]
        // v 3: [0.142255127429962, -0.0136603936553001, -0.550647497177124]  [1, 0, 0]
      }

      // if (navBall!=null)
      // {
      //   // dumpGuiObj(navBall.transform);
      //   var sf=navBall.transform.Find("NavBall/frame");
      //   var m=sf.GetComponent<MeshFilter>().sharedMesh;
      //   print(m.vertices.Length+" verts "+m.triangles.Length/3.0f+" triangles");
      //   for (int i=0; i<m.vertices.Length; ++i)
      //   {
      //     var p=navBall.transform.InverseTransformPoint(sf.TransformPoint(m.vertices[i]));
      //     print("v "+i+": "+(Vector3d)p+"  "+(Vector3d)(Vector3)m.uv[i]);
      //   }
      //   print("tex: "+sf.renderer.material.mainTexture+" shader: "+sf.renderer.material.shader.name);
      // }

      // print("background: "+myText.textStyle.normal.background);

      // if (navBall!=null) print("pos: "+(Vector3d)navBall.transform.position+" scale: "+(Vector3d)navBall.transform.lossyScale);
      // print("speed: "+DumpObjectProps(t.textStyle));
      // print("myText: "+DumpObjectProps(myText.textStyle));
      // if (t!=null) print("pos: "+t.transform.position+" ofs: "+t.offset+" scale: "+t.transform.lossyScale);
    }
  }
}


//ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ//


} // namespace
