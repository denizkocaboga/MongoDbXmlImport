using System;
using System.Xml.Serialization;

namespace Challenge
{
    [Serializable]
    [XmlType("contact")]
    public class XmlContact
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlElement("phone")]
        public string Phone { get; set; }
    }
}