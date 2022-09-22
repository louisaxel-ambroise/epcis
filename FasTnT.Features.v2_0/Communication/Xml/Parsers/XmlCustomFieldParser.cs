﻿using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model.Events;
using System.Globalization;

namespace FasTnT.Features.v2_0.Communication.Xml.Parsers;

public static class XmlCustomFieldParser
{
    public static Field ParseCustomFields(XElement element, FieldType fieldType)
    {
        var field = new Field
        {
            Type = fieldType,
            Name = element.Name.LocalName,
            Namespace = string.IsNullOrWhiteSpace(element.Name.NamespaceName) ? default : element.Name.NamespaceName,
            TextValue = element.HasElements ? default : element.Value,
            NumericValue = element.HasElements ? default : float.TryParse(element.Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB"), out float floatValue) ? floatValue : default(float?),
            DateValue = element.HasElements ? default : DateTime.TryParse(element.Value, out DateTime dateValue) ? dateValue : default(DateTime?)
        };

        field.Children.AddRange(element.Elements().Select(x => ParseCustomFields(x, fieldType)));
        field.Children.AddRange(element.Attributes().Where(x => !x.IsNamespaceDeclaration).Select(ParseAttribute));

        return field;
    }

    public static Field ParseCustomFields(XAttribute element, FieldType fieldType)
    {
        return new()
        {
            Type = fieldType,
            Name = element.Name.LocalName,
            Namespace = string.IsNullOrWhiteSpace(element.Name.NamespaceName) ? default : element.Name.NamespaceName,
            TextValue = element.Value,
            NumericValue = float.TryParse(element.Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB"), out float floatValue) ? floatValue : default(float?),
            DateValue = DateTime.TryParse(element.Value, out DateTime dateValue) ? dateValue : default(DateTime?)
        };
    }

    public static Field ParseAttribute(XAttribute element)
    {
        return new()
        {
            Type = FieldType.Attribute,
            Name = element.Name.LocalName,
            Namespace = element.Name.NamespaceName,
            TextValue = element.Value,
            NumericValue = float.TryParse(element.Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB"), out float floatValue) ? floatValue : default(float?),
            DateValue = DateTime.TryParse(element.Value, out DateTime dateValue) ? dateValue : default(DateTime?)
        };
    }
}