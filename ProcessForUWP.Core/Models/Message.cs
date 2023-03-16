using Newtonsoft.Json;

namespace ProcessForUWP.Core.Models
{
    internal class Message
    {
        public int ID { get; set; }
        public string Package { get; set; }
        public MessageType MessageType { get; set; }

        private readonly JsonSerializerSettings jSetting = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
        };

        public Message(MessageType typeEnum, int id = 0, object msg = null)
        {
            ID = id;
            SetPackage(msg);
            MessageType = typeEnum;
        }

        public static Message MakeMessage(MessageType typeEnum, int id = 0, object mag = null)
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
