using System.Linq;
using System.Xml;

namespace JackCompiler
{
    public class XmlConverter
    {
        public XmlDocument ConvertTokens(params Token[] tokens)
        {
            var doc = new XmlDocument();
            var root = doc.CreateElement("tokens");
            doc.AppendChild(root);
            foreach (var token in tokens)
            {
                CreateElement(root, token);
            }
            return doc;
        }

        public XmlDocument ConvertNode(NodeBase tree)
        {
            var doc = new XmlDocument();
            CreateElement(doc, tree);
            return doc;
        }

        private static XmlElement CreateElement(XmlNode parent, NodeBase nodeBase)
        {
            XmlDocument doc = parent is XmlDocument ? (XmlDocument)parent : parent.OwnerDocument;
            var el = doc.CreateElement(nodeBase.Type.GetAttribute<ElementNameAttribute, NodeType>().Name);
            if (nodeBase is Token)
            {
                el.AppendChild(doc.CreateTextNode(((Token)nodeBase).Value));
            }
            if (nodeBase is Identifier)
            {
                var id = nodeBase as Identifier;
                el.SetAttribute("kind", id.Kind.ToString().ToLower());
                if (id.Number.HasValue) el.SetAttribute("number", id.Number.ToString());
                el.SetAttribute("isDefinition", id.IsDefinition.ToString().ToLower());
            }
            if (nodeBase is Node)
            {
                Node n = (Node)nodeBase;
                if (!n.Children.Any())
                    el.AppendChild(doc.CreateTextNode("\n"));
                foreach (var child in n.Children)
                    CreateElement(el, child);
            }
            parent.AppendChild(el);
            return el;
        }

    }
}
