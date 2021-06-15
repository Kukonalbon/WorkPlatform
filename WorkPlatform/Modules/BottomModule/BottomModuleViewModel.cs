using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;
using PlatformCommon.ViewModel;
using PlatformCommon.Events;
using System.Collections.ObjectModel;
using PlatformCommon.Plugin;

namespace Modules.BottomModule
{
    [Export(typeof(BottomModuleViewModel))]
    public class BottomModuleViewModel : BaseObject
    {

        private ObservableCollection<IPluginObject> pluginObjects = new ObservableCollection<IPluginObject>();
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
        public BottomModuleViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<PluginsEvent>().Subscribe(ProjectChangeChanged);
        }

        void ProjectChangeChanged(PluginsEventArgs args)
        {
            switch (args.Action)
            {
                case PluginAction.Add:
                    {

                        break;
                    }

                case PluginAction.Show:
                    {
                        if (args.PluginObject.IsTool && !PluginObjects.Contains(args.PluginObject))
                        {
                            PluginObjects.Add(args.PluginObject);
                        }
                        break;
                    }

                case PluginAction.Hide:
                    {

                        break;
                    }

                case PluginAction.Close:
                    {
                        if (args.PluginObject.IsTool)
                        {
                            PluginObjects.Remove(args.PluginObject);
                        }
                        break;
                    }

                default: break;
            }


        }
    }

}
