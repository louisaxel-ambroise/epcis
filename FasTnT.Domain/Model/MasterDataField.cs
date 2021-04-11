using System.Collections.Generic;

namespace FasTnT.Domain.Model
{
    public class MasterDataField
    {
        public MasterDataAttribute Attribute { get; set; }
        public string Name { get; set; }
        public string Namespace { get; set; }
        public string Value { get; set; }
        public List<MasterDataField> Children { get; set; } = new();
    }
}
