using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentTests.Utitlities
{
    public static class XmlSerializer
    {
        private static readonly Dictionary<Type, System.Xml.Serialization.XmlSerializer> serialisers = new Dictionary<Type, System.Xml.Serialization.XmlSerializer>();

        public static string XmlSerialize<T>(this T objectToSerialise) where T : class, new()
        {
            System.Xml.Serialization.XmlSerializer serialiser;
            var type = typeof(T);
            if (!serialisers.ContainsKey(type))
            {
                serialiser = new System.Xml.Serialization.XmlSerializer(type);
                serialisers.Add(type, serialiser);
            }
            else
            {
                serialiser = serialisers[type];
            }

            string xml;
            using (var writer = new StringWriter())
            {
                serialiser.Serialize(writer, objectToSerialise);
                xml = writer.ToString();
            }

            return xml;
        }

        public static T XmlDeserialize<T>(this string xml) where T : class, new()
        {
            System.Xml.Serialization.XmlSerializer serialiser;
            var type = typeof(T);
            if (!serialisers.ContainsKey(type))
            {
                serialiser = new System.Xml.Serialization.XmlSerializer(type);
                serialisers.Add(type, serialiser);
            }
            else
            {
                serialiser = serialisers[type];
            }
            T newObject;

            using (var reader = new StringReader(xml))
            {
                try { newObject = (T)serialiser.Deserialize(reader); }
                catch { return null; }
            }

            return newObject;
        }
    }
}
