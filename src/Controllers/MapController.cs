using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapService.Controllers
{
    public class MapController : Controller
    {
        [HttpGet("/MapServer")]
        public IActionResult Map(string lyr, int x, int y, int z)
        {
            var bytes = new MapServer().Request(lyr, x, y, z);
            if (bytes == null) return new EmptyResult();
            return File(bytes, "image/png");
        }

        [HttpGet("/help")]
        public IActionResult Help()
        {
            var str = @"
s 卫星
t 地形
m 行政
v 矢量
*h 带标签
 调用信息： /MapServer?lyr=m&x=0&y=2&z=2
"; return Content(str);
        }
    }
}
