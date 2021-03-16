namespace LazySloth.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public static class MethodsHelper
    {
        public static HashSet<FieldInfo> GetNonStaticFieldInfos(Type type)
        {
            var fieldInfos = new HashSet<FieldInfo>();
            fieldInfos = GetAllFields(type, fieldInfos, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return fieldInfos;
        }

        private static HashSet<FieldInfo> GetAllFields(Type type, HashSet<FieldInfo> fieldInfos, BindingFlags bindingAttr)
        {
            fieldInfos = GetMemberInfos(type, fieldInfos, bindingAttr);
            return fieldInfos;
        }

        private static HashSet<T> GetMemberInfos<T>(Type type, HashSet<T> memberInfos, BindingFlags bindingAttr)
            where T : MemberInfo
        {
            var allMembers = type.GetMembers(bindingAttr);
            foreach (MemberInfo memberInfo in allMembers)
            {
                var info = memberInfo as T;
                if (info != null && info.DeclaringType == type)
                {
                    memberInfos.Add(info);
                }
            }

            if (type.BaseType != typeof(object))
            {
                GetMemberInfos(type.BaseType, memberInfos, bindingAttr);
            }

            return memberInfos;
        }
    }
}