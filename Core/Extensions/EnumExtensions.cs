using System.Reflection;

namespace System;

public static class EnumExtensions
{
    public static T? GetAttribute<T>(this Enum value) 
        where T : Attribute
    {
        return value.GetType().GetMember(value.ToString()).First().GetCustomAttribute<T>();
    }
}