namespace NLib.Dde
{
    using System;

    /// <summary>
    /// This is the base class for all NDde event argument classes.
    /// </summary>
    public abstract class DdeEventArgs : EventArgs
    {
        /// <summary>
        /// This returns a string containing the current values of all properties.
        /// </summary>
        /// <returns>
        /// A string containing the current values of all properties.
        /// </returns>
        public override string ToString()
        {
            string s = "";
            foreach (System.Reflection.PropertyInfo property in this.GetType().GetProperties())
            {
                if (s.Length == 0)
                {
                    s += property.Name + "=" + property.GetValue(this, null).ToString();
                }
                else
                {
                    s += " " + property.Name + "=" + property.GetValue(this, null).ToString();
                }
            }
            return s;
        }

    } // class

} // namespace