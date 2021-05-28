using FasTnT.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FasTnT.Domain.Utils
{
    public abstract class Enumeration : IComparable
    {
        public string DisplayName { get; }
        public short Id { get; }

        protected Enumeration() { }

        protected Enumeration(short id, string displayName)
        {
            Id = id;
            DisplayName = displayName;
        }

        public static IEnumerable<T> GetAll<T>() where T : Enumeration, new() => typeof(T).GetTypeInfo().GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).Select(x => x.GetValue(new T())).Cast<T>();
        public static T GetByDisplayName<T>(string displayName) where T : Enumeration, new() => GetAll<T>().SingleOrDefault(x => x.DisplayName == displayName) ?? throw NameException<T>(displayName);
        public static T GetByDisplayNameInvariant<T>(string displayName) where T : Enumeration, new() => GetAll<T>().SingleOrDefault(x => string.Equals(x.DisplayName, displayName, StringComparison.OrdinalIgnoreCase)) ?? throw NameException<T>(displayName);
        public static T GetById<T>(short id) where T : Enumeration, new() => GetAll<T>().SingleOrDefault(x => x.Id == id) ?? throw IdException<T>(id);

        public int CompareTo(object obj) => Id.CompareTo(((Enumeration)obj).Id);
        public override int GetHashCode() => 2108858624 + GetType().GetHashCode() + Id.GetHashCode();
        public override string ToString() => DisplayName;
        public override bool Equals(object obj) => (obj is Enumeration other) && GetType().Equals(obj.GetType()) && Id.Equals(other.Id);

        private static Exception NameException<T>(string displayName) => new EpcisException(ExceptionType.ValidationException, $"Invalid value for {typeof(T).Name} : '{displayName}'");
        private static Exception IdException<T>(short id) => new EpcisException(ExceptionType.ValidationException, $"Invalid ID for {typeof(T).Name} : {id}");

        public static bool operator ==(Enumeration left, Enumeration right) => left is null ? right is null : left.Equals(right);
        public static bool operator !=(Enumeration left, Enumeration right) => !(left == right);

        public static bool operator <(Enumeration left, Enumeration right) => left is null ? right is not null : left.CompareTo(right) < 0;
        public static bool operator <=(Enumeration left, Enumeration right) => left is null || left.CompareTo(right) <= 0;
        public static bool operator >(Enumeration left, Enumeration right) => left is not null && left.CompareTo(right) > 0;
        public static bool operator >=(Enumeration left, Enumeration right) => left is null ? right is null : left.CompareTo(right) >= 0;
    }
}
