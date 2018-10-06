﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ASCIIsome.Commands
{
    public class RubberDuckCommand : ICommand
    {
        public ViewModel CurrentViewModel { get; set; }
        public RubberDuckCommand(ViewModel currentViewModel) => CurrentViewModel = currentViewModel;

        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            Debug.WriteLine("RubberDuck command executed. ");
        }
    }
}
