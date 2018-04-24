using System.Collections.Generic;
using System.Reflection;
using System;
using System.Windows.Documents;

namespace TestWPF
{
    public struct EnumField<T, R>
    {
        string _name;
        R _id;
        T _value;

        public string Name { get { return _name; } }
        public R Id { get { return _id; } }
        public T Value { get { return _value; } }

        public EnumField(string name, R id, T type)
        {
            _name = name;
            _id = id;
            _value = type;
        }
    }

    public static class EnumHelper
    {
        public static List<EnumField<T, R>> GetEnumFields<T, R>()
        {
            Type t = typeof(T);
            if (!t.IsEnum) return null;

            FieldInfo[] fields = t.GetFields(BindingFlags.Static | BindingFlags.Public);
            List<EnumField<T, R>> result = new List<EnumField<T, R>>(fields.Length);

            for (int i = 0; i < fields.Length; ++i)
            {
                Object o = fields[i].GetValue(null);
                result.Add(new EnumField<T, R>(fields[i].Name, (R)o, (T)o));
            }

            return result;
        }
    }
}