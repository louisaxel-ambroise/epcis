using FasTnT.Domain.Utils;

namespace FasTnT.Domain.Enumerations
{
    public class EpcType : Enumeration
    {
        public static readonly EpcType List = new EpcType(0, "list");
        public static readonly EpcType ParentId = new EpcType(1, "parentID");
        public static readonly EpcType InputQuantity = new EpcType(2, "inputQuantity");
        public static readonly EpcType OutputQuantity = new EpcType(3, "outputQuantity");
        public static readonly EpcType InputEpc = new EpcType(4, "inputEPC");
        public static readonly EpcType OutputEpc = new EpcType(5, "outputEPC");
        public static readonly EpcType Quantity = new EpcType(6, "quantity");
        public static readonly EpcType ChildEpc = new EpcType(7, "childEPC");
        public static readonly EpcType ChildQuantity = new EpcType(8, "childQuantity");

        public EpcType() { }
        public EpcType(short id, string displayName) : base(id, displayName) { }
    }
}
