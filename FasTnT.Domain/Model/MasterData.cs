using System.Collections.Generic;

namespace FasTnT.Domain.Model
{
    public class MasterData
    {
        public Request Request { get; set; }
        public string Type { get; set; }
        public string Id { get; set; }

        public List<MasterDataHierarchy> Hierarchies { get; set; } = new();
        public List<MasterDataAttribute> Attributes { get; set; } = new();
        public List<MasterDataChildren> Children { get; set; } = new();
    }
}
