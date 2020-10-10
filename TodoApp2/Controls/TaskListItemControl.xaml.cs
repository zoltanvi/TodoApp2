using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Input;
using TodoApp2.Core;
using System.Windows;

namespace TodoApp2
{
    /// <summary>
    /// Interaction logic for TaskListItemControl.xaml
    /// </summary>
    public partial class TaskListItemControl : UserControl
    {
        private static readonly FontFamily ConsolasFont = new FontFamily("Consolas");
        private static readonly SolidColorBrush HighLightColor = new SolidColorBrush(Color.FromRgb(78, 201, 176));

        public TaskListItemControl()
        {
            InitializeComponent();
        }
        
        private void TaskDescriptionTextBlock_OnTargetUpdated(object sender, DataTransferEventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                bool formatted = false;
                string text = textBlock.Text;

                // Create the textBlock inline collection from scratch
                textBlock.Inlines.Clear();

                // Iterate the text as a character array
                // When the current char is a format char, save the text buffer
                // into the inline collection with the corresponding formatting
                // and switch formatting on / off
                // So basically this only switches the formatting flag on/off and writes the text 
                StringBuilder stringBuilder = new StringBuilder();
                foreach (char character in text)
                {
                    if (character != TextBoxPreviewKeyDownHelper.FormatCharacter)
                    {
                        stringBuilder.Append(character);
                    }
                    else
                    {
                        if (stringBuilder.Length > 0)
                        {
                            AddInline(textBlock, stringBuilder, ref formatted);
                        }
                        formatted = !formatted;
                    }
                }

                AddInline(textBlock, stringBuilder, ref formatted);
            }
        }

        // private helper to add the StringBuilder content to the textBlock with correct formatting
        private void AddInline(TextBlock destination, StringBuilder stringBuilder, ref bool formatted)
        {
            if (formatted)
            {
                Run inline = new Run(stringBuilder.ToString())
                {
                    FontFamily = ConsolasFont,
                    Foreground = HighLightColor
                };
                destination.Inlines.Add(inline);
            }
            else
            {
                destination.Inlines.Add(new Run(stringBuilder.ToString()));
            }

            stringBuilder.Clear();
        }

        /// <summary>
        /// Preview the input into the message box and respond as required
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(DataContext is TaskListItemViewModel viewModel)
            {
                TextBoxPreviewKeyDownHelper.TextBox_PreviewKeyDown(sender, e, viewModel.UpdateContent);
            }
        }
    }
}
