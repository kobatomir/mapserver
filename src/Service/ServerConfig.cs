using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapService
{
    class ServerConfig
    {
        /// <summary>
        /// 卫星地址
        /// </summary>
        public string SatellitePath { set; get; }

        /// <summary>
        /// 地形地址
        /// </summary>
        public string TerrainPath { set; get; }

        /// <summary>
        /// 电子
        /// </summary>
        public string LinePath { set; get; }

        /// <summary>
        /// 标签
        /// </summary>
        public string LabelPath { set; get; }

        /// <summary>
        /// 矢量
        /// </summary>
        public string VectorPath { set; get; }

        /// <summary>
        /// 卫星地址
        /// </summary>
        public string SatelliteName { set; get; }

        /// <summary>
        /// 地形地址
        /// </summary>
        public string TerrainName { set; get; }

        /// <summary>
        /// 电子
        /// </summary>
        public string LineName { set; get; }

        /// <summary>
        /// 标签
        /// </summary>
        public string LabelName { set; get; }

        /// <summary>
        /// 矢量
        /// </summary>
        public string VectorName { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsNoMapShowLabel { set; get; }
    }

    static class ServerConfigInstance{
       public  static ServerConfig Instance { set; get; }
    }
}
