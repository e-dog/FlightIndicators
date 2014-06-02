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
    // var f=KSP.IO.TextWriter.CreateForType<FlightIndicatorsGUI>("testfile.txt", null);
    // f.WriteLine("Writing a line!");
    // f.WriteLine("Another one");
    // f.Close();

    // var r=KSP.IO.TextReader.CreateForType<FlightIndicatorsGUI>("testfile.txt", null);
    // print("Text length: "+r.ReadToEnd().Length);
    // r.Close();
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
    // const float cx=121.10f*scale;
    const float cy=-104.54f*scale, cr=120.23f*scale;
    const float y0=-7*scale, y1=-194*scale;

    float ty=(y1-y0)*y+y0;

    float dy=Mathf.Abs(ty-cy);
    dy+=(0.5f-x)*14*scale;
    if (dy<0) dy=0;

    float xo=cr*cr-dy*dy;
    if (xo>=0) xo=cr-Mathf.Sqrt(xo);
    else xo=cr;

    var o=new GameObject("KzFlightIndicators");
    o.transform.parent=frame;
    o.transform.localPosition=new Vector3((x*frameOffset+8)*scale+xo, ty, 0);
    o.transform.localRotation=Quaternion.identity;
    o.transform.localScale=Vector3.one;
    o.layer=layer;

    var t=o.AddComponent<ScreenSafeGUIText>();
    t.text="";
    t.textSize=12;
    if (x<0.5f) t.textSize=10;
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
    o.transform.localPosition=new Vector3(-(frameOffset+137)*scale, 0.22493f, 1.0f);
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
  }
}


//ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ//


} // namespace
