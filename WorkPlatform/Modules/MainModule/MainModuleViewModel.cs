using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;
using WorkCommon.Plugin;
using WorkCommon.ViewModel;

namespace Modules.MainModule
{
    [Export(typeof(MainModuleViewModel))]
    public class MainModuleViewModel : BaseObject
    {

        private ObservableCollection<IPluginObject> pluginObjects;
        public ObservableCollection<IPluginObject> PluginObjects
        {
            get
            {
                return pluginObjects;
            }
            set
            {
                pluginObjects = value;
            }
        }

        [ImportingConstructor]
        public MainModuleViewModel(IEventAggregator eventAggregator)
        {
            InitPlugins();
        }

        private void InitPlugins()
        {
            PluginObjects = PluginManager.Instance.PluginObjects;
        }

    }

}
