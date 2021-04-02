using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Formatter.Xml
{
    public static class XElementExtensions
    {
        public static void AddIfNotNull(this XElement destination, XElement children)
        {
            if (children == null || children.IsEmpty) return;

            destination.Add(children);
        }

        public static void AddIfNotNull(this XElement destination, IEnumerable<XElement> children)
        {
            if (children == null || !children.Any()) return;
            if (children.All(x => x.IsEmpty)) return;

            destination.Add(children.Where(x => !x.IsEmpty));
        }
    }
}
