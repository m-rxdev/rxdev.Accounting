﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace rxdev.Accounting.App.Views
{
    /// <summary>
    /// Interaction logic for BankTransactionGridView.xaml
    /// </summary>
    public partial class BankTransactionGridView : UserControl
    {
        public BankTransactionGridView()
        {
            InitializeComponent();
        }

        private void DataGrid_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }
    }
}
