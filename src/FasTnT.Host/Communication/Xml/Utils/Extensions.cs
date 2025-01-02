namespace FasTnT.Host.Communication.Xml.Utils;

public static class Extensions
{
    public static string ToUpperString(this object value) => value.ToString().ToUpperInvariant();

    public static void AddIfNotNull(this XElement destination, XElement children)
    {
        if (children != null && (!children.IsEmpty || children.HasAttributes))
        {
            destination.Add(children);
        }
    }
    public static void AddIfNotNull(this XElement destination, XAttribute attribute)
    {
        if (attribute != null && !string.IsNullOrEmpty(attribute.Value))
        {
            destination.Add(attribute);
        }
    }

    public static void AddIfNotNull(this XElement destination, IEnumerable<XElement> children)
    {
        if (children != null && children.Any(x => !x.IsEmpty || x.HasAttributes))
        {
            destination.Add(children.Where(x => !x.IsEmpty || x.HasAttributes));
        }
    }
}
