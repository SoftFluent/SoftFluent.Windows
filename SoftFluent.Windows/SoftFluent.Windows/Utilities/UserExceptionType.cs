namespace SoftFluent.Windows.Utilities
{
    /// <summary>
    /// Defines exceptions for end user, for message customizing purposes.
    /// </summary>
    public enum UserExceptionType
    {
        /// <summary>
        /// Supports the CodeFluent infrastructure and is not intended to be used directly from your code. 
        /// </summary>
        Undefined,

        /// <summary>
        /// An item with the same key has already been added.
        /// Argument {0} should represent the item's key value.
        /// </summary>
        ItemAlreadyAdded,

        /// <summary>
        /// An item with an empty or null collection key cannot be added.
        /// Argument {0} should represent the item's key value.
        /// </summary>
        VoidCollectionKey,
    }
}
