using System;
using System.Text;


namespace Keramzit {


//ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ//


public static class Utils
{
  public static string format_distance(double d)
  {
    var a=Math.Abs(d);
    if (a>1e12)
      return (d*1e-9).ToString("n0")+"Gm";
    else if (a>1e9)
      return (d*1e-6).ToString("n0")+"Mm";
    else if (a>1e6)
      return (d*1e-3).ToString("n0")+"km";
    else if (a>100)
      return d.ToString("n0")+"m";
    else
      return d.ToString("n3")+"m";
  }


  public static string format_timespan(double s)
  {
    var a=Math.Abs(s);
    if (a<60)
      return s.ToString("n2");
    else if (a<3600)
    {
      var m=Math.Floor(s/60); s-=m*60;
      return string.Format("{0:n0}:{1:00.00}", m, s);
    }
    else
    {
      //== format year and day, respect Kerbin/Earth time setting
      var h=Math.Floor(s/3600); s-=h*3600;
      var m=Math.Floor(s/60); s-=m*60;
      return string.Format("{0:n0}:{1:00}:{2:00.00}", h, m, s);
    }
  }
}


//ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ//


} // namespace
