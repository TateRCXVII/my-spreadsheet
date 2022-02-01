namespace Extensions
{
    /// <summary>
    /// Extension class for the evaluator class
    /// 
    /// Contains extensions to check if stack is empty and to check if an operator is on top.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Extension for checking if a value is present at the top of the stack
        /// </summary>
        /// <typeparam name="T"> int or string value in stack</typeparam>
        /// <param name="stack">stack to be checked</param>
        /// <param name="value">the value being checked</param>
        /// <returns> true if the value is on top, false otherwise</returns>
        public static bool HasOnTop<T>(this Stack<T> stack, T value)
        {
            if (!stack.IsEmpty() && stack.Peek().Equals(value))
                return true;
            return false;

        }

        /// <summary>
        /// Extension to check if the stack is empty
        /// </summary>
        /// <typeparam name="T">Type in the stack</typeparam>
        /// <param name="stack">Stack to be checked</param>
        /// <returns>true if stack is empty, false otherwise </returns>
        public static bool IsEmpty<T>(this Stack<T> stack)
        {
            return stack.Count == 0;
        }
    }
}