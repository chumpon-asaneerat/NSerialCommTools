namespace NLib.Dde.Advanced
{
    using System;
    using NLib.Dde.Foundation.Advanced;

    /// <summary>
    /// This defines a transaction filter.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use a transaction filter to intercept the DDEML callback function.  The <c>PreFilterTransaction</c> method will be called every time the
    /// DDEML callback function executes.  The <c>Transaction</c> object passed into the method contains the parameters of the DDE callback
    /// function.  By using a transaction filter the developer has compelete control over the DDEML.  See the MSDN documentation for more
    /// information on using the DDEML.
    /// </para>
    /// <para>
    /// <note type="caution">
    /// Incorrect usage of the DDEML can cause this library to function incorrectly and can lead to resource leaks.
    /// </note>
    /// </para>
    /// </remarks>
    public interface IDdeTransactionFilter
    {
        /// <summary>
        /// This filters a transaction before it is dispatched.
        /// </summary>
        /// <param name="t">
        /// The transaction to be dispatched.
        /// </param>
        /// <returns>
        /// True to filter the transaction and stop it from being dispatched, false otherwise.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method is called everytime the DDEML callback function executes.
        /// </para>
        /// <para>
        /// <note type="caution">
        /// Incorrect usage of the DDEML can cause this library to function incorrectly and can lead to resource leaks.
        /// </note>
        /// </para>
        /// </remarks>
        bool PreFilterTransaction(DdeTransaction t);

    } // interface


} // namespace