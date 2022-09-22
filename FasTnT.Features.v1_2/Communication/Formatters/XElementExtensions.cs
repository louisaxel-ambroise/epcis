namespace FasTnT.Features.v1_2.Communication.Formatters;

public static class XElementExtensions
{
    public static void AddIfNotNull(this XElement destination, XElement children)
    {
        if (children != null && !children.IsEmpty)
        {
            destination.Add(children);
        }
    }

    public static void AddIfNotNull(this XElement destination, XAttribute children)
    {
        if (children != null && !string.IsNullOrEmpty(children.Value))
        {
            destination.Add(children);
        }
    }

    public static void AddIfNotNull(this XElement destination, IEnumerable<XElement> children)
    {
        if (children != null && children.Any(x => !x.IsEmpty))
        {
            destination.Add(children.Where(x => !x.IsEmpty));
        }
    }
}
