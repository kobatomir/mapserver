using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapService
{
    public class MapServer
    {
        static object lockobj = new object();
        public async Task<byte[]> Request(string type, int nX, int nY, int nZ)
        {
            nZ++;
            var bBuff = (byte[])null;
            var bBuff1 = type[0] switch
            {
                's' =>await GetMap(nY, nX, nZ, ServerConfigInstance.Instance.SatellitePath, ServerConfigInstance.Instance.SatelliteName),
                't' =>await GetMap(nY, nX, nZ, ServerConfigInstance.Instance.TerrainPath, ServerConfigInstance.Instance.TerrainName),
                'm' =>await GetMap(nY, nX, nZ, ServerConfigInstance.Instance.LinePath, ServerConfigInstance.Instance.LineName),
                'v' =>await GetMap(nY, nX, nZ, ServerConfigInstance.Instance.VectorPath, ServerConfigInstance.Instance.VectorName),
                _ => null
            };
            var bBuff2 = type.Contains("h") ?await GetMap(nY, nX, nZ, ServerConfigInstance.Instance.LabelPath, ServerConfigInstance.Instance.LabelName) : null;
            var hash = type.Contains("s") || type.Contains("t") || type.Contains("v") || type.Contains("m");
            if (hash && !type.Contains("h")) bBuff = bBuff1;
            else if (hash && type.Contains("h"))
            {
                if (bBuff1 != null && bBuff2 == null)
                    bBuff = bBuff1;
                else if (bBuff1 != null)
                    bBuff = this.MeldMap(bBuff1, bBuff2);
                else if (ServerConfigInstance.Instance.IsNoMapShowLabel)
                    bBuff = bBuff2;
            }
            else if (bBuff2 != null)
                bBuff = bBuff2;
            return bBuff;
        }


        private async Task<byte[]> GetMap(int nRow, int nCol, int nZoom, string strFilePath, string strFileName)
        {
            var buffer1 = (byte[])null;
            var buffer2 = new byte[20];
            var dirctory = GetDirctory(strFilePath, nZoom);
            if (dirctory.Count <= 0) return null;
            foreach (var path1 in dirctory)
            {
                try
                {
                    if (!Directory.Exists(path1)) continue;
                    var path2 = Path.Combine(path1, strFileName + ".idx");
                    if (!File.Exists(path2)) continue;
                    var path3 = Path.Combine(path1, "range.txt");
                    if (!File.Exists(path3)) continue;
                    var strArray =(await File.ReadAllTextAsync(path3)).Split(',');
                    var longi1 = Convert.ToDouble(strArray[0]);
                    var lati = Convert.ToDouble(strArray[1]);
                    var longi2 = Convert.ToDouble(strArray[2]);
                    var mercatorLatitude1 = Longlatcal.GetMercatorLatitude(Convert.ToDouble(strArray[3]), nZoom - 1);
                    var mercatorLongitude1 = Longlatcal.GetMercatorLongitude(longi1, nZoom - 1);
                    var mercatorLatitude2 = Longlatcal.GetMercatorLatitude(lati, nZoom - 1);
                    var mercatorLongitude2 = Longlatcal.GetMercatorLongitude(longi2, nZoom - 1);
                    if (mercatorLatitude1 > nRow || nRow > mercatorLatitude2 ||
                        (mercatorLongitude1 > nCol || nCol > mercatorLongitude2)) continue;
                    var offset1 = (long)(mercatorLongitude2 - mercatorLongitude1 + 1) * (long)(nRow - mercatorLatitude1) * 16L + (long)(nCol - mercatorLongitude1) * 16L;
                    if (offset1 < 0L) continue;
                    long offset2 = 0;
                    var count = 0;
                    short num = 0;
                    lock (lockobj)
                    {
                        var fileStream = (FileStream)null;
                        try
                        {
                            fileStream = File.Open(path2, FileMode.Open, FileAccess.Read);
                            fileStream.Seek(offset1, SeekOrigin.Begin);
                            fileStream.Read(buffer2, 0, 8);
                            offset2 = BitConverter.ToInt64(buffer2, 0);
                            fileStream.Read(buffer2, 0, 4);
                            count = BitConverter.ToInt32(buffer2, 0);
                            fileStream.Read(buffer2, 0, 2);
                            num = BitConverter.ToInt16(buffer2, 0);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        finally
                        {
                            fileStream?.Close();
                        }
                    }
                    if (count < 100) return null;
                    var path4 = Path.Combine(path1, strFileName + num.ToString() + ".dat");
                    lock (lockobj)
                    {
                        FileStream fileStream = null;
                        try
                        {
                            fileStream = File.Open(path4, FileMode.Open, FileAccess.Read);
                            fileStream.Seek(offset2, SeekOrigin.Begin);
                            buffer1 = new byte[count];
                            if (fileStream.Read(buffer1, 0, count) != count) buffer1 = null;
                            break;
                        }catch(Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        finally
                        {
                            fileStream?.Close();
                        }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return buffer1;
        }

        private byte[] MeldMap(byte[] bBuff1, byte[] bBuff2)
        {
            try
            {
                MemoryStream memoryStream1 = new();
                MemoryStream memoryStream2 = new(bBuff1);
                MemoryStream memoryStream3 = new(bBuff2);
                var image1 = Image.FromStream(memoryStream2);
                var image2 = Image.FromStream(memoryStream3);
                memoryStream2?.Close();
                memoryStream3?.Close();
                var graphics = Graphics.FromImage(image1);
                graphics.DrawImage(image2, 0, 0, 256, 256);
                image1.Save(memoryStream1, ImageFormat.Jpeg);
                var buffer = memoryStream1.GetBuffer();
                memoryStream1.Close();
                image1.Dispose();
                image2.Dispose();
                graphics.Dispose();
                return buffer;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }



        private List<string> GetDirctory(string strServerAddress, int nZoom)
        {
            List<string> stringList = new();
            var strArray = strServerAddress.Split(';');
            foreach (var t1 in strArray)
            {
                if (!Directory.Exists(t1)) continue;
                var directories = Directory.GetDirectories(t1, nZoom.ToString() + "*");
                stringList.AddRange(from t in directories let fileName = Path.GetFileName(t) let length = fileName.IndexOf("-", StringComparison.Ordinal) where length <= 0 || fileName?[..length] == nZoom.ToString() select t);
            }
            return stringList;
        }

    }
}
