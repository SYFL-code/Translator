using System;

namespace Translator;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal class CreditsAttribute : Attribute
{
    public string author { get; }

    public CreditsAttribute(string author)
    {
        this.author = author;
    }
}