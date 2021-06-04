﻿using FasTnT.Domain.Utils;

namespace FasTnT.Domain.Enumerations
{
    public class ContactInformationType : Enumeration
    {
        public static readonly ContactInformationType Sender = new(0, "Sender");
        public static readonly ContactInformationType Receiver = new(1, "Receiver");

        public ContactInformationType() { }
        public ContactInformationType(short id, string displayName) : base(id, displayName) { }
    }
}