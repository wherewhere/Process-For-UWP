using Newtonsoft.Json;

namespace ProcessForUWP.Core.Models
{
    public class Message
    {
        public int ID { get; set; }
        public string Package { get; set; }
        public ControlType ControlType { get; set; }

        private readonly JsonSerializerSettings jSetting = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
        };

        public Message(ControlType typeEnum, int id = 0, object msg = null)
        {
            ID = id;
            SetPackage(msg);
            ControlType = typeEnum;
        }

        public static Message MakeMessage(ControlType typeEnum, int id = 0, object mag = null)
        {
            return new Message(typeEnum, id, mag);
        }

        public void SetPackage(object msg)
        {
            Package = JsonConvert.SerializeObject(msg, jSetting);
        }

        public T GetPackage<T>()
        {
            return JsonConvert.DeserializeObject<T>(Package, jSetting);
        }
    }
}
