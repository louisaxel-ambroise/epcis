using FasTnT.Domain.Utils;

namespace FasTnT.Domain.Enumerations
{
    public class FieldType : Enumeration
    {
        public static readonly FieldType Ilmd = new FieldType(0, "Ilmd");
        public static readonly FieldType CustomField = new FieldType(1, "CustomField");
        public static readonly FieldType Extension = new FieldType(2, "Extension");
        public static readonly FieldType BaseExtension = new FieldType(3, "BaseExtension");
        public static readonly FieldType ErrorDeclarationExtension = new FieldType(4, "ErrorDeclarationExtension");
        public static readonly FieldType ErrorDeclarationCustomField = new FieldType(5, "ErrorDeclarationCustomField");
        public static readonly FieldType IlmdExtension = new FieldType(6, "IlmdExtension");
        public static readonly FieldType BusinessLocationCustomField = new FieldType(7, "BusinessLocationCustomField");
        public static readonly FieldType BusinessLocationExtension = new FieldType(8, "BusinessLocationExtension");
        public static readonly FieldType ReadPointCustomField = new FieldType(9, "ReadPointCustomField");
        public static readonly FieldType ReadPointExtension = new FieldType(10, "ReadPointExtension");
        public static readonly FieldType Attribute = new FieldType(11, "Attribute");

        public FieldType() { }
        public FieldType(short id, string displayName) : base(id, displayName) { }
    }
}
