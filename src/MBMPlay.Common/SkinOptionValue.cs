using System.Xml.Serialization;

namespace MBMPlay.Common;

public abstract class SkinOptionValue
{
	public abstract SkinOptionType SkinOptionType { get; }

	[XmlAttribute("id")]
	public virtual string ID { get; set; }

	[XmlText]
	public virtual string Value { get; set; }
}
