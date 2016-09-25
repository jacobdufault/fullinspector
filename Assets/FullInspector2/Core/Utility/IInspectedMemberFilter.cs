namespace FullInspector {
    /// <summary>
    /// The inspected member filter allows you to specify which items on the inspected type you
    /// are interested in. Items that the filter is not interested in are not returned. There are
    /// some common filters that you may be interested in at InspectedMemberFilters.
    /// </summary>
    /// <remarks>
    /// Filters are an important performance abstraction. The results are cached and then reused
    /// reused hundreds of times over.
    /// </remarks>
    public interface IInspectedMemberFilter {
        /// <summary>
        /// Are we interested in this property?
        /// </summary>
        bool IsInterested(InspectedProperty property);

        /// <summary>
        /// Are we interested in this method?
        /// </summary>
        bool IsInterested(InspectedMethod method);
    }
}