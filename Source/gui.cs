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
  List<ScreenSafeGUIText> leftText, rightText;


  public void Awake()
  {
  }


  public static string DumpObjectFields(object obj)
  {
      BindingFlags flags =
        BindingFlags.Public |
        BindingFlags.Instance |
        BindingFlags.GetField |
        BindingFlags.GetProperty |
        BindingFlags.FlattenHierarchy;

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
      BindingFlags flags =
        BindingFlags.Public |
        BindingFlags.Instance |
        BindingFlags.GetField |
        BindingFlags.GetProperty |
        BindingFlags.FlattenHierarchy;

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


  void dumpMesh(Transform sf, Transform navBall)
  {
    print("*** mesh for "+sf.name);
    var m=sf.GetComponent<MeshFilter>().sharedMesh;
    print(m.vertices.Length+" verts "+m.triangles.Length/3.0f+" triangles");
    for (int i=0; i<m.vertices.Length; ++i)
    {
      var p=navBall.InverseTransformPoint(sf.TransformPoint(m.vertices[i]));
      print("v "+i+": "+(Vector3d)p+"  "+(Vector3d)m.vertices[i]+"  "+(Vector3d)(Vector3)m.uv[i]);
    }
    for (int i=0; i<m.triangles.Length; ++i)
      print("ind "+m.triangles[i]);
    print("tex: "+sf.renderer.material.mainTexture+" shader: "+sf.renderer.material.shader.name);
    print("tex ofs: "+sf.renderer.material.mainTextureOffset+sf.renderer.material.mainTextureScale+" color: "+sf.renderer.material.color);
    print("pos: "+(Vector3d)sf.localPosition);
    print("rot: "+(Vector3d)sf.localEulerAngles);
    print("scl: "+(Vector3d)sf.localScale);
    // print("status: "+sf.gameObject.activeInHierarchy+sf.renderer.enabled+sf.renderer.isVisible);

    // print("gameobj: "+DumpObjectProps(sf.gameObject));
    // print("renderer: "+DumpObjectProps(sf.renderer));
  }


  public void onDestroy()
  {
    if (leftText!=null)
    {
      foreach (var t in leftText) UnityEngine.Object.Destroy(t.gameObject);
      leftText=null;
    }

    if (rightText!=null)
    {
      foreach (var t in rightText) UnityEngine.Object.Destroy(t.gameObject);
      rightText=null;
    }
  }


  const int layer=12;
  const float scale=0.001115242428169431f;
  const float frameOffset=150;


  ScreenSafeGUIText makeText(float x, float y, GUIStyle style, Transform frame)
  {
    const float cx=121.10f*scale, cy=-104.54f*scale, cr=120.23f*scale;
    const float y0=-7*scale, y1=-194*scale;

    var o=new GameObject("KzFlightIndicators");
    o.transform.parent=frame;
    o.transform.localPosition=new Vector3((x*frameOffset+8)*scale, y*(y1-y0)+y0, 0);
    o.transform.localRotation=Quaternion.identity;
    o.transform.localScale=Vector3.one;
    o.layer=layer;

    var t=o.AddComponent<ScreenSafeGUIText>();
    t.text="";
    t.textSize=12;
    t.textStyle=style;

    return t;
  }


  void buildGui()
  {
    if (leftText!=null) return;

    var fmc=FlightUIModeController.Instance;
    if (fmc==null) return;

    var navBall=fmc.navBall;
    if (navBall==null) return;

    var srcText=FlightUIController.fetch.speed;

    // frame
    var o=new GameObject("KzFlightIndicatorsFrame");
    var frame=o.transform;
    o.transform.parent=navBall.transform;
    o.transform.localPosition=new Vector3(-(frameOffset+134)*scale, 0.22493f, 1.0f);
    o.transform.localRotation=Quaternion.identity;
    o.transform.localScale=Vector3.one;
    o.layer=layer;

    float w=256*scale;
    float h=211*scale;

    var m=new Mesh();
    m.vertices=new[]
    {
      new Vector3(0f, 0f, 0f),
      new Vector3( w, 0f, 0f),
      new Vector3(0f, -h, 0f),
      new Vector3( w, -h, 0f),
      // new Vector3(0.00000f,  0.00000f, 0f),
      // new Vector3(0.28153f,  0.00000f, 0f),
      // new Vector3(0.00000f, -0.23859f, 0f),
      // new Vector3(0.28153f, -0.23859f, 0f),
    };

    m.uv=new[]
    {
      new Vector2(0, 1),
      new Vector2(1, 1),
      new Vector2(0, 0),
      new Vector2(1, 0),
    };

    m.triangles=new[]
    {
      0, 1, 2,
      3, 2, 1,
    };

    m.RecalculateNormals();
    m.RecalculateBounds();

    o.AddComponent<MeshFilter>().mesh=m;
    o.AddComponent<MeshRenderer>();
    o.renderer.material=new Material(navBall.transform.Find("NavBall/frame").renderer.material);
    o.renderer.material.mainTexture=GameDatabase.Instance.GetTexture("FlightIndicators/frame", false);
    o.renderer.castShadows=false;
    o.renderer.receiveShadows=false;

    // text
    leftText=new List<ScreenSafeGUIText>();
    rightText=new List<ScreenSafeGUIText>();

    var  leftStyle=new GUIStyle(srcText.textStyle);
    var rightStyle=new GUIStyle(srcText.textStyle);
    leftStyle .alignment=TextAnchor.MiddleLeft;
    rightStyle.alignment=TextAnchor.MiddleRight;

    for (int i=0; i<12; ++i)
    {
      var t=makeText(0, (i+0.5f)/12, leftStyle, frame);
      t.text="Periapsis";
      leftText.Add(t);

      t=makeText(1, (i+0.5f)/12, rightStyle, frame);
      t.text=i+"0654Gm";
      rightText.Add(t);
    }
  }


  public void LateUpdate()
  {
    buildGui();

    //== update text

    /*
    var fmc=FlightUIModeController.Instance;
    if (fmc!=null)
    {
      var navBall=fmc.navBall;
      if (navBall!=null && myText==null)
      {
        var srcText=FlightUIController.fetch.speed;
        const int layer=12;

        // var fo=navBall.transform.Find("NavBall/frame").gameObject;
        // var oo=GameObject.Instantiate(fo) as GameObject;
        // oo.name="testobj";
        // oo.transform.parent=fo.transform.parent;
        // oo.transform.localPosition=fo.transform.localPosition+new Vector3(-0.01f, 0f, 0f);
        // oo.transform.localRotation=fo.transform.localRotation;
        // oo.transform.localScale=fo.transform.localScale;
        // oo.renderer.material.mainTexture=GameDatabase.Instance.GetTexture("FlightIndicators/frame", false);

        // frame
        var o=new GameObject("KzFlightIndicatorsFrame");
        o.transform.parent=navBall.transform;
        o.transform.localPosition=new Vector3(-0.28f, 0.22493f, 1.0f);
        o.transform.localRotation=Quaternion.identity;
        o.transform.localScale=Vector3.one;
        // o.transform.localPosition=new Vector3(0.000676377210766077f-0.01f, -0.00270877708680928f, -0.250294268131256f);
        // o.transform.localEulerAngles=new Vector3(90, 180, 0);
        // o.transform.localScale=new Vector3(0.127970084547997f, 0.101707048714161f, 0.10845036059618f);
        o.layer=layer;

        var m=new Mesh();
        m.vertices=new[]
        {
          // new Vector3( 0.5f, 0f, -0.5f),
          // new Vector3(-0.5f, 0f, -0.5f),
          // new Vector3( 0.5f, 0f,  0.5f),
          // new Vector3(-0.5f, 0f,  0.5f),
          new Vector3(0.00000f,  0.00000f, 0f),
          new Vector3(0.28153f,  0.00000f, 0f),
          new Vector3(0.00000f, -0.23859f, 0f),
          new Vector3(0.28153f, -0.23859f, 0f),
        };

        m.uv=new[]
        {
          new Vector2(0, 1),
          new Vector2(1, 1),
          new Vector2(0, 0),
          new Vector2(1, 0),
        };

        m.triangles=new[]
        {
          0, 1, 2,
          3, 2, 1,
        };

        m.RecalculateNormals();
        m.RecalculateBounds();

        o.AddComponent<MeshFilter>().mesh=m;
        o.AddComponent<MeshRenderer>();
        o.renderer.material=new Material(navBall.transform.Find("NavBall/frame").renderer.material);
        o.renderer.material.mainTexture=GameDatabase.Instance.GetTexture("FlightIndicators/frame", false);
        o.renderer.castShadows=false;
        o.renderer.receiveShadows=false;

        print("frame mesh created!");

        //==
        // oo.GetComponent<MeshFilter>().mesh=m;
        // oo.renderer.material=o.renderer.sharedMaterial;

        //==

        // shader: "Unlit/Transparent Tint"

        // navBall frame relative to navBall:
        // v 0: [-0.139279067516327, 0.224930435419083, -0.55064731836319]  [0, 1, 0]
        // v 1: [0.142255127429962, 0.224930435419083, -0.550647377967834]  [1, 1, 0]
        // v 2: [-0.139279067516327, -0.0136603936553001, -0.550647377967834]  [0, 0, 0]
        // v 3: [0.142255127429962, -0.0136603936553001, -0.550647497177124]  [1, 0, 0]

        // text
        o=new GameObject("KzFlightIndicators");
        o.transform.parent=navBall.transform;
        o.transform.localPosition=new Vector3(-0.38f, 0.22493f, -0.5f);
        o.transform.localRotation=Quaternion.identity;
        o.transform.localScale=Vector3.one;
        o.layer=layer;
        // o.transform.localPosition=new Vector3(0.1000f, -0.15658f, 0.10f);
        // o.transform.localRotation=Quaternion.identity;
        // o.transform.localPosition=new Vector3(0.000676377210766077f-0.01f, -0.00270877708680928f, -0.250294268131256f);
        // o.transform.localEulerAngles=new Vector3(90, 180, 0);
        // o.transform.localScale=new Vector3(0.127970084547997f, 0.101707048714161f, 0.10845036059618f);

        myText=o.AddComponent<ScreenSafeGUIText>();
        myText.text="";
        for (int i=0; i<12; ++i) myText.text+="Periapsis 987654 Gm\n";
        myText.textSize=12;
        myText.textStyle=new GUIStyle(srcText.textStyle);
        myText.textStyle.alignment=TextAnchor.UpperLeft;

        // dumpGuiObj(navBall.transform);
      }

      if (navBall!=null)
      {
        // dumpGuiObj(navBall.transform);
        // dumpMesh(navBall.transform.Find("NavBall/frame"), navBall.transform);
        // dumpMesh(navBall.transform.Find("NavBall/testobj"), navBall.transform);
        // dumpMesh(navBall.transform.Find("KzFlightIndicatorsFrame"), navBall.transform);
      }

      // print("background: "+myText.textStyle.normal.background);

      // if (navBall!=null) print("pos: "+(Vector3d)navBall.transform.position+" scale: "+(Vector3d)navBall.transform.lossyScale);
      // print("speed: "+DumpObjectProps(t.textStyle));
      // print("myText: "+DumpObjectProps(myText.textStyle));
      // if (t!=null) print("pos: "+t.transform.position+" ofs: "+t.offset+" scale: "+t.transform.lossyScale);
    }
    */
  }
}


//ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ//


} // namespace
