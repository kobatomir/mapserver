using System;

namespace MapService
{
  public class Longlatcal
  {
    public static int GetMercatorLongitude(double lon, int zoom)
    {
      if (lon == 180.0)
        lon -= 1E-09;
      if (lon > 180.0)
        lon -= 360.0;
      if (lon < -180.0)
        lon += 360.0;
      return (int) Math.Floor((lon + 180.0) / 360.0 * Math.Pow(2.0, (double) zoom));
    }

    public static double GetLongitudeByColumn(int nCol, int nZoom)
    {
      var num = (double) nCol / Math.Pow(2.0, (double) nZoom) * 360.0 - 180.0;
      if (num > 180.0)
        num = 360.0 - num;
      if (num < -180.0)
        num += 360.0;
      return num;
    }

    public static int GetMercatorLatitude(double lati, int zoom)
    {
      const double pi = Math.PI;
      var num1 = lati switch
      {
        >= 85.0511287798066 => 85.0511287798066,
        <= -85.0511287798066 => -85.0511287798066,
        _ => lati
      };
      if (num1 > 90.0)
        num1 -= 180.0;
      if (num1 < -90.0)
        num1 += 180.0;
      var a = Math.PI * num1 / 180.0;
      var num2 = 0.5 * Math.Log((1.0 + Math.Sin(a)) / (1.0 - Math.Sin(a)));
      var num3 = Math.Pow(2.0, (double) zoom);
      return (int) Math.Floor((1.0 - num2 / pi) / 2.0 * num3);
    }

    public static double GetLatitudeByRow(int nRow, int nZoom)
    {
      var num = Math.Exp((1.0 - (double) nRow / Math.Pow(2.0, (double) nZoom) * 2.0) * Math.PI * 2.0);
      return Math.Asin((num - 1.0) / (num + 1.0)) * 180.0 / Math.PI;
    }
  }
}
