using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace RadioNoise
{
    public class TextSearchFilter
    {
        public TextSearchFilter(
            ICollectionView filterView,
            TextBox textbox)
        {
            string filterText = "";

            filterView.Filter = delegate(object obj)
            {
                if (String.IsNullOrEmpty(filterText))
                    return true;

                Stream stream = obj as Stream;
                if (stream == null)
                    return false;

                String streamText = stream.Title + stream.Genre + stream.Description;

                int index = streamText.IndexOf(filterText, 0, StringComparison.InvariantCultureIgnoreCase);

                return index > -1;
            };

            textbox.TextChanged += delegate
            {
                filterText = textbox.Text;
                filterView.Refresh();
            };
        }
    }
}
