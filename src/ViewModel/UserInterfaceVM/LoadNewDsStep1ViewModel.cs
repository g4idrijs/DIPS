﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DIPS.ViewModel.Commands;
using Microsoft.Win32;

namespace DIPS.ViewModel.UserInterfaceVM
{
    public class LoadNewDsStep1ViewModel : BaseViewModel
    {
        public ICommand ProgressToStep2Command { get; set; }
        public ICommand OpenFileDialogCommand { get; set; }

        private ObservableCollection<FileInfo> _listofFiles;

        public ObservableCollection<FileInfo> ListOfFiles
        {
            get { return _listofFiles; }
            set
            {
                _listofFiles = value; 
            }
        }

        private ObservableCollection<String> _listOfFileNames;

        public ObservableCollection<String> ListOfFileNames
        {
            get { return _listOfFileNames; }
            set
            {
                _listOfFileNames = value;
                OnPropertyChanged();
            }
        }
        

        public LoadNewDsStep1ViewModel()
        {
            SetupCommands();
        }

        private void SetupCommands()
        {
            ProgressToStep2Command = new RelayCommand(new Action<object>(ConfirmAndMoveToStep2));
            OpenFileDialogCommand = new RelayCommand(new Action<object>(SelectFilesForDataset));
        }

        private void ConfirmAndMoveToStep2(object obj)
        {
            if (ValidateFields())
            {
                _LoadNewDsStep2ViewModel.ListOfFiles = ListOfFiles;
                OverallFrame.Content = _LoadNewDsStep2ViewModel;
            }
            
        }

        private void SelectFilesForDataset(object obj)
        {
            Stream myStream;
            OpenFileDialog dialogOpen = new OpenFileDialog();
            ;
            //Setup properties for open file dialog
            dialogOpen.InitialDirectory = "C:\\";
            dialogOpen.Filter = @"Bitmaps|*.bmp|Jpgs|*.jpg";
            dialogOpen.FilterIndex = 1;
            dialogOpen.Multiselect = true;
            dialogOpen.Title = "Please select image files which are going to be part of this dataset";

            Nullable<bool> isOkay = dialogOpen.ShowDialog();
            String[] strFiles = dialogOpen.FileNames;

            if (isOkay == true)
            {
                ListOfFiles = new ObservableCollection<FileInfo>();
                ListOfFileNames = new ObservableCollection<string>();

                foreach (string file in dialogOpen.FileNames)
                {
                    ListOfFileNames.Add(file);

                    FileInfo uploadFile = new FileInfo(file);
                    ListOfFiles.Add(uploadFile);
                }
            }

        }
        private Boolean ValidateFields()
        {
            if (ListOfFiles.Count == 0)
            {
                MessageBox.Show("No files have been selected for processing.", "No files selected.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            return true;
        }
    }
}
