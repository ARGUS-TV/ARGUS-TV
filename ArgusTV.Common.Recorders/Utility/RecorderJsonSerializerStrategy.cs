using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgusTV.Common.Recorders.Utility
{
    public class RecorderJsonSerializerStrategy : PocoJsonSerializerStrategy
    {
        private static readonly long _initialJavaScriptDateTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
        private static readonly DateTime _minimumJavaScriptDate = new DateTime(100, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override object DeserializeObject(object value, Type type)
        {
            bool isString = value is string;

            Type valueType = null;
            bool isEnum = type.IsEnum || (IsNullable(type, out valueType) && valueType.IsEnum);
            if (isEnum && (isString || value is Int32 || value is Int64))
            {
                if (!isString
                    || Enum.IsDefined(valueType ?? type, value))
                {
                    return Enum.Parse(valueType ?? type, value.ToString());
                }
            }
            else if (type == typeof(DateTime))
            {
                var s = value as string;
                if (s != null
                    && s.StartsWith("/Date(", StringComparison.Ordinal) && s.EndsWith(")/", StringComparison.Ordinal))
                {
                    int tzCharIndex = s.IndexOfAny(new char[] { '+', '-' }, 7);
                    long javaScriptTicks = Convert.ToInt64(s.Substring(6, (tzCharIndex > 0) ? tzCharIndex - 6 : s.Length - 8));
                    DateTime time = new DateTime((javaScriptTicks * 10000) + _initialJavaScriptDateTicks, DateTimeKind.Utc);
                    if (tzCharIndex > 0)
                    {
                        time = time.ToLocalTime();
                    }
                    return time;
                }
            }
            return base.DeserializeObject(value, type);
        }

        protected override bool TrySerializeKnownTypes(object input, out object output)
        {
            if (input is DateTime)
            {
                DateTime value = (DateTime)input;
                DateTime time = value.ToUniversalTime();

                string suffix = "";
                if (value.Kind != DateTimeKind.Utc)
                {
                    TimeSpan localTZOffset;
                    if (value >= time)
                    {
                        localTZOffset = value - time;
                        suffix = "+";
                    }
                    else
                    {
                        localTZOffset = time - value;
                        suffix = "-";
                    }
                    suffix += localTZOffset.ToString("hhmm");
                }

                if (time < _minimumJavaScriptDate)
                {
                    time = _minimumJavaScriptDate;
                }
                long ticks = (time.Ticks - _initialJavaScriptDateTicks) / (long)10000;
                output = "/Date(" + ticks + suffix + ")/";
                return true;
            }
            return base.TrySerializeKnownTypes(input, out output);
        }

        private static bool IsNullable(Type type, out Type valueType)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                valueType = type.GetGenericArguments()[0];
                return true;
            }

            valueType = null;
            return false;
        }
    }
}
