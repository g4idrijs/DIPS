﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Database.Repository;
using DIPS.Database.Objects;
using DIPS.Processor.Client;
using DIPS.Unity;
using DIPS.ViewModel.Commands;
using DIPS.Util.Extensions;
using Microsoft.Practices.Unity;
using System.Diagnostics;
using DIPS.Util.Commanding;
using Database.Objects;

namespace DIPS.ViewModel.UserInterfaceVM
{
    public class LoadNewDsStep2ViewModel : BaseViewModel, IPipelineInfo
    {
        #region Properties
        private ObservableCollection<FileInfo> _listOfFiles;

        public ObservableCollection<FileInfo> ListOfFiles
        {
            get { return _listOfFiles; }
            set
            {
                _listOfFiles = value;
                OnPropertyChanged();
            }
        }

        public IProcessingService Service { get; set; }

        private ObservableCollection<Technique> _listOfTechniques;

        public ObservableCollection<Technique> ListofTechniques
        {
            get { return _listOfTechniques; }
            set
            {
                _listOfTechniques = value;
                OnPropertyChanged();
            }
        }

        public Technique ChosenTechnique
        {
            get
            {
                return _chosenTechnique;
            }
            set
            {
                _chosenTechnique = value;
                Log.ProcessID = value.ID;
                _updateAlgorithmsInTechnique(value);
                OnPropertyChanged();
            }
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Technique _chosenTechnique;

        private ObservableCollection<AlgorithmViewModel> _techniqueAlgorithms;

        public ObservableCollection<AlgorithmViewModel> TechniqueAlgorithms
        {
            get { return _techniqueAlgorithms; }
            set
            {
                _techniqueAlgorithms = value;
                OnPropertyChanged();
            }
        }

        public AlgorithmViewModel SelectedAlgorithm
        {
            get
            {
                return _selectedAlgorithm;
            }
            set
            {
                _selectedAlgorithm = value;
                OnPropertyChanged();
            }
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private AlgorithmViewModel _selectedAlgorithm;

        private AlgorithmViewModel _selectedTechSelectedItem;

        public AlgorithmViewModel SelectedTechSelectedItem
        {
            get { return _selectedTechSelectedItem; }
            set
            {
                _selectedTechSelectedItem = value; 
                OnPropertyChanged();
            }
        }
        
        public RelayCommand ProgressToStep3Command { get; set; }
        public ICommand BuildAlgorithmCommand { get; set; }
        public ICommand GoBackCommand { get; set; }
        public UnityCommand LoadFromFile
        {
            get;
            set;
        }

        string IPipelineInfo.PipelineName
        {
            get;
            set;
        }

        string IPipelineInfo.PipelineID
        {
            get;
            set;
        }

        ObservableCollection<AlgorithmViewModel> IPipelineInfo.SelectedProcesses
        {
            get { return TechniqueAlgorithms; }
        }
        #endregion

        #region Constructor
        public LoadNewDsStep2ViewModel()
        {
            ListofTechniques = new ObservableCollection<Technique>();
            ImageProcessingRepository imgProRep = new ImageProcessingRepository();

            ListofTechniques = imgProRep.getAllTechnique();

            TechniqueAlgorithms = new ObservableCollection<AlgorithmViewModel>();
            SetupCommands();
        } 
        #endregion

        #region Methods
        private bool _canProgressToStep3(object obj)
        {
            return TechniqueAlgorithms.Any();
        }

        private void ProgressToStep3(object obj)
        {
            OverallFrame.Content = BaseViewModel._PostProcessingViewModel;

            BaseViewModel._LoadNewDsStep3ViewModel.ListOfFiles.Clear();
            this.ListOfFiles.ForEach( BaseViewModel._LoadNewDsStep3ViewModel.ListOfFiles.Add );
            BaseViewModel._LoadNewDsStep3ViewModel.PipelineAlgorithms.Clear();
            TechniqueAlgorithms.ForEach( _addAlgorithmToStep3 );

            if (ChosenTechnique != null)
            {
                BaseViewModel._LoadNewDsStep3ViewModel.PipelineName = ChosenTechnique.Name;
            }
        }

        private void _addAlgorithmToStep3( AlgorithmViewModel viewModel )
        {
            viewModel.IsRemovable = false;
            BaseViewModel._LoadNewDsStep3ViewModel.PipelineAlgorithms.Add( viewModel );
        }


        private bool _canBuildAlgorithm(object obj)
        {
            return GlobalContainer.Instance.Container.Contains<IPipelineManager>();
        }

        private void BuildAlgorithm(object obj)
        {
            OverallFrame.Content = BaseViewModel._AlgorithmBuilderViewModel;

            _AlgorithmBuilderViewModel.Container = GlobalContainer.Instance.Container;
            _AlgorithmBuilderViewModel.FromLoadStep2 = true;
            _AlgorithmBuilderViewModel.FromViewAlgorithms = false;
            _AlgorithmBuilderViewModel.GoBackButtonState = System.Windows.Visibility.Visible;
            _AlgorithmBuilderViewModel.UseAlgorithmButtonState = System.Windows.Visibility.Visible;
            _AlgorithmBuilderViewModel.ListOfFiles = ListOfFiles;

            IPipelineManager manager = GlobalContainer.Instance.Container.Resolve<IPipelineManager>();
            _AlgorithmBuilderViewModel.AvailableAlgorithms.Clear();
            _AlgorithmBuilderViewModel.SelectedProcesses.Clear();
            _AlgorithmBuilderViewModel.PipelineID = string.Empty;

            foreach (var algorithm in manager.AvailableProcesses)
            {
                AlgorithmViewModel viewModel = new AlgorithmViewModel(algorithm);
                viewModel.IsRemovable = false;
                _AlgorithmBuilderViewModel.AvailableAlgorithms.Add(viewModel);
            }
        }

        private void SetupCommands()
        {
            ProgressToStep3Command = new RelayCommand(new Action<object>(ProgressToStep3), _canProgressToStep3);
            BuildAlgorithmCommand = new RelayCommand(new Action<object>(BuildAlgorithm), _canBuildAlgorithm);
            GoBackCommand = new RelayCommand(new Action<object>(_goBack));
            LoadFromFile = new LoadPipelineCommand(this);
            LoadFromFile.Container = GlobalContainer.Instance.Container;

            TechniqueAlgorithms.CollectionChanged += (s, e) => ProgressToStep3Command.ExecutableStateChanged();
        }

        private void _goBack(object obj)
        {
            OverallFrame.Content = _LoadNewDsStep1ViewModel;
        }

        private void _updateAlgorithmsInTechnique(Technique value)
        {
            TechniqueAlgorithms.Clear();
            if (value != null)
            {
                IUnityContainer c = GlobalContainer.Instance.Container;
                IPipelineManager manager = c.Resolve<IPipelineManager>();

                var restoredPipeline = manager.RestorePipeline(value.xml);
                TechniqueAlgorithms.Clear();
                
                foreach (var process in restoredPipeline)
                {
                    TechniqueAlgorithms.Add(new AlgorithmViewModel(process));
                }
            }
        } 
        #endregion


        
    }
}
