namespace NLib.Dde.Foundation.Advanced
{
    using System;

    internal interface IDdemlTransactionFilter
    {
        bool PreFilterTransaction(DdemlTransaction t);

    } // interface

} // namespace