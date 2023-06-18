﻿using System.Windows;
using System.Windows.Controls;
using TodoApp2.Core;

namespace TodoApp2
{
    public class TagControl : Label
    {
        public static readonly DependencyProperty TagTitleProperty = DependencyProperty.Register(nameof(TagTitle), typeof(string), typeof(TagControl), new PropertyMetadata());
        public static readonly DependencyProperty TagBodyProperty = DependencyProperty.Register(nameof(TagBody), typeof(string), typeof(TagControl), new PropertyMetadata());
        public static readonly DependencyProperty TagStyleProperty = DependencyProperty.Register(nameof(TagStyle), typeof(MessageType), typeof(TagControl), new PropertyMetadata());

        public string TagTitle
        {
            get { return (string)GetValue(TagTitleProperty); }
            set { SetValue(TagTitleProperty, value); }
        }

        public string TagBody
        {
            get { return (string)GetValue(TagBodyProperty); }
            set { SetValue(TagBodyProperty, value); }
        }

        public MessageType TagStyle
        {
            get { return (MessageType)GetValue(TagStyleProperty); }
            set { SetValue(TagStyleProperty, value); }
        }
    }
}
