using Utilitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PlainWpf.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class XmlTreeViewModel : BindableBase
    {
        /// <summary>
        /// 
        /// </summary>
        public XmlTreeViewModel()
        {
            treeView = new TreeView();
        }

        private ItemCollection treeViewItems;
        /// <summary>
        /// 
        /// </summary>
        public ItemCollection TreeViewItems { get { return treeViewItems; } set { SetProperty(ref treeViewItems, value); } }

        private DelegateCommand<string[]> dropFile;
        /// <summary>
        /// 
        /// </summary>
        public DelegateCommand<string[]> DropFile { get { return dropFile ?? (dropFile = new DelegateCommand<string[]>(OnDropFile)); } }

        private void OnDropFile(string[] uris)
        {
            var xmlReader = new CsXmlCommentReader(filePath: uris[0], readElement: OnXmlElement, endElement: OnEndXmlElement, readAttribute: OnXmlAttribute, readText: OnXmlText);
            xmlReader.Read();
            TreeViewItems = treeView.Items;
        }

        private TreeView treeView;
        private TreeViewItem treeViewItem;

        private void OnXmlElement(string elementName)
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = elementName;
            if (treeViewItem == null)
            {
                treeView.Items.Add(item);
            }
            else
            {
                treeViewItem.Items.Add(item);
            }
            treeViewItem = item;
        }

        private void OnEndXmlElement(string elementName)
        {
            if (treeViewItem != null && treeViewItem.Header != null && treeViewItem.Header.Equals(elementName))
            {
                treeViewItem = treeViewItem.Parent as TreeViewItem;
            }
        }

        private void OnXmlAttribute(string name, string value)
        {
            if (treeViewItem != null)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = name + "='" + value + "'";
                treeViewItem.Items.Add(item);
            }
        }

        private void OnXmlText(string value)
        {
            if (treeViewItem != null)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = "\"" + value.Trim() + "\"";
                treeViewItem.Items.Add(item);
            }
        }
    }
}
