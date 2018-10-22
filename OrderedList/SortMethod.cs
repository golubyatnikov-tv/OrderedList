namespace gt.Collections.Ordered
{
    public enum SortMethod
    {
        /// <summary>
        ///  If value is already present in collection, the insertion point will be before (to the left of) any existing entries
        /// </summary>
        BisectRight,
        /// <summary>
        ///  If value is already present in collection, the insertion point will be after (to the right of) any existing entries
        /// </summary>
        BisectLeft
    }
}