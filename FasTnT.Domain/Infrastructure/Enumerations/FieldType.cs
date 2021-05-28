using FasTnT.Domain.Utils;

namespace FasTnT.Domain.Enumerations
{
    public class FieldType : Enumeration
    {
        public static readonly FieldType Ilmd = new(0, "Ilmd");
        public static readonly FieldType CustomField = new(1, "CustomField");
        public static readonly FieldType Extension = new(2, "Extension");
        public static readonly FieldType BaseExtension = new(3, "BaseExtension");
        public static readonly FieldType ErrorDeclarationExtension = new(4, "ErrorDeclarationExtension");
        public static readonly FieldType ErrorDeclarationCustomField = new(5, "ErrorDeclarationCustomField");
        public static readonly FieldType IlmdExtension = new(6, "IlmdExtension");
        public static readonly FieldType BusinessLocationCustomField = new(7, "BusinessLocationCustomField");
        public static readonly FieldType BusinessLocationExtension = new(8, "BusinessLocationExtension");
        public static readonly FieldType ReadPointCustomField = new(9, "ReadPointCustomField");
        public static readonly FieldType ReadPointExtension = new(10, "ReadPointExtension");
        public static readonly FieldType Attribute = new(11, "Attribute");

        public FieldType() { }
        public FieldType(short id, string displayName) : base(id, displayName) { }
    }
}
