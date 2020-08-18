using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace MeisterTask
{
    class CacheItem
    {
        public DateTime ValidTill { get; private set; }
        public List<dynamic> Content { get; private set; }
        public dynamic ContentSingle { get { return Content[0]; } }

        public CacheItem(int secondsToLive, List<dynamic> content)
        {
            Content = content;
            ValidTill = DateTime.Now.AddSeconds(secondsToLive);
        }

        public CacheItem(int secondsToLive, dynamic content)
        {
            Content = new List<dynamic>();
            Content.Add(content);
            ValidTill = DateTime.Now.AddSeconds(secondsToLive);
        }

        public bool IsValid()
        {
            return ValidTill > DateTime.Now;
        }
    }
}
