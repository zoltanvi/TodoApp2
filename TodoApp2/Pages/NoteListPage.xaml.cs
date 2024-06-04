﻿using Modules.Common.Views.Pages;
using TodoApp2.Core;

namespace TodoApp2
{
    public partial class NoteListPage : GenericBasePage<NoteListPageViewModel>
    {
        public NoteListPage(NoteListPageViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}