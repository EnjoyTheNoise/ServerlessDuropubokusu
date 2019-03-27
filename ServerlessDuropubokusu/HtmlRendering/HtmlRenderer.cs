using System;
using ServerlessDuropubokusu.Models;

namespace ServerlessDuropubokusu.HtmlRendering
{
    public class HtmlRenderer
    {
        private readonly HtmlContent _content = new HtmlContent();

        public HtmlRenderer CreateContent()
        {
            _content.Content += "<html><body></body></html>";
            return this;
        }

        public HtmlRenderer AddNode(INode field, bool isSasNeeded)
        {
            var index = _content.Content.IndexOf("<body>", StringComparison.Ordinal) + "<body>".Length;

            var nodes = "<div>";

            var sas = "";
            if (isSasNeeded)
            {
                var fileModel = field as FileModel;
                sas = fileModel?.Sas;
            }

            nodes += $"<p>{field.Name}: <a href=\"{field.Uri + sas}\">Get Item</a></p>";

            nodes += "</div>";

            _content.Content = _content.Content.Insert(index, nodes);

            return this;
        }

        public string Build()
        {
            return _content.Content;
        }
    }
}
