using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Assets.Scripts.Learning
{
    public static class Serializer
    {
        public static void Serialize(object obj, string path)
        {
            using(var stream = File.Create(path))
            {
                var binaryformatter = new BinaryFormatter();
                binaryformatter.Serialize(stream, obj);
            }
        }

        public static T Deserialize<T>(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                var binaryformatter = new BinaryFormatter();
                var des = binaryformatter.Deserialize(stream);
                return (T)des;
            }
        }
    }
}
