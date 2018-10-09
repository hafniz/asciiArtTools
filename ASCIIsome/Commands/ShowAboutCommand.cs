﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ASCIIsome.Commands
{
    public class ShowAboutCommand : ICommand
    {
        public ViewModel CurrentViewModel { get; set; }
        public ShowAboutCommand(ViewModel currentViewModel) => CurrentViewModel = currentViewModel;

        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter)
        {
            if (!Application.Current.Windows.OfType<About>().Any())
            {
                new About().Show();
            }
            else
            {
                Application.Current.Windows.OfType<About>().Single().Activate();
            }
        }
    }
}
