using FasTnT.Domain.Utils;

namespace FasTnT.Domain.Enumerations
{
    public class EpcType : Enumeration
    {
        public static readonly EpcType List = new(0, "list");
        public static readonly EpcType ParentId = new(1, "parentID");
        public static readonly EpcType InputQuantity = new(2, "inputQuantity");
        public static readonly EpcType OutputQuantity = new(3, "outputQuantity");
        public static readonly EpcType InputEpc = new(4, "inputEPC");
        public static readonly EpcType OutputEpc = new(5, "outputEPC");
        public static readonly EpcType Quantity = new(6, "quantity");
        public static readonly EpcType ChildEpc = new(7, "childEPC");
        public static readonly EpcType ChildQuantity = new(8, "childQuantity");

        public EpcType() { }
        public EpcType(short id, string displayName) : base(id, displayName) { }
    }
}
