using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgusTV.Common.Recorders.Utility
{
    public class RecorderJsonSerializer : RestSharp.Serializers.ISerializer
    {
        public RecorderJsonSerializer()
		{
			ContentType = "application/json";
		}

		public string Serialize(object obj)
		{
			return SimpleJson.SerializeObject(obj, new RecorderJsonSerializerStrategy());
		}

		public string DateFormat { get; set; }
		public string RootElement { get; set; }
		public string Namespace { get; set; }
		public string ContentType { get; set; }
    }
}
