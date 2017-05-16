using System;

namespace Jockusch.Common
{
  public static class CustomEscapings
  {
    public static CustomEscaping Attribute {get;} = new AttributeEscaping();
    /// <summary>Replace spaces with underscores. As much as possible, leave everything else alone.</summary>
    public static CustomEscaping HumanFriendlyXToEmpty {get;} = new HumanFriendlyQuoteEscaping(true);
    public static CustomEscaping HumanFriendlyCantHandleEmptyString { get;} = new HumanFriendlyQuoteEscaping(false);

    public static CustomEscaping Json { get; } = new JsonEscaping();
    public static CustomEscaping DatasetSave { get;} = new DatasetFileSaveEscaping();
    public static CustomEscaping SingleQuotesOnly {get;} = new SingleQuotesOnlyEscaping();
    /// <summary>Not always reversible.</summary>
    public static CustomEscaping HumanFriendlyFilenameManyToOne { get; } = new HumanFriendlyFilenameEscaping();
  }
}

