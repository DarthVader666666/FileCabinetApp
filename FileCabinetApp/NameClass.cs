using System;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Contains first and last name.
    /// </summary>
    public class NameClass
    {
        /// <summary>
        /// Gets or sets first name.
        /// </summary>
        /// <value>string.</value>
        [XmlAttribute("first")]
        public string First { get; set; }

        /// <summary>
        /// Gets or sets last name.
        /// </summary>
        /// <value>string.</value>
        [XmlAttribute("last")]
        public string Last { get; set; }
    }
}
