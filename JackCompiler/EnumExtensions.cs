using System;
using System.Linq;

namespace JackCompiler
{
    public static class EnumExtensions
    {
        public static T GetAttribute<T, TE>(this TE source)
        {
            var enumType = typeof(TE);
            var memberInfos = enumType.GetMember(source.ToString());
            var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
            var valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(T), false);
            return (T)valueAttributes[0];
        }
    }
}
