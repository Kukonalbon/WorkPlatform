using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.MefExtensions;
using Microsoft.Practices.ServiceLocation;
using PlatformCommon.Behaviors;
using PlatformCommon.Manager;
using PlatformCommon.Manager.LayoutMgr;
using ClientManager;
using Jisons;
using Modules.BottomModule;
using Modules.MainModule;

namespace WorkPlatform
{
    public class WorkBootstrapper : MefBootstrapper
    {
        /// <summary>
        /// 对日志组件的包装类
        /// </summary>
        private readonly LoggerWarp loggerWarp = new LoggerWarp();

        /// <summary> 用于加载起始页面、设置主窗体 </summary>
        protected override void InitializeShell()
        {
            base.InitializeShell();

            ServiceLocator.Current.GetInstance<ILayoutManager>().CombineViewPart();

            Application.Current.MainWindow = (Shell)this.Shell;
            Application.Current.MainWindow.Show();
        }

        /// <summary>
        /// 重写区域加载器,获取区域
        /// </summary>
        /// <returns></returns>
        protected override Microsoft.Practices.Prism.Regions.IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            var factory = base.ConfigureDefaultRegionBehaviors();
            factory.AddIfMissing("AutoPopulateExportedViewsBehavior", typeof(AutoPopulateExportedViewsBehavior));
            return factory;
        }

        protected override Microsoft.Practices.Prism.Logging.ILoggerFacade CreateLogger()
        {
            return loggerWarp;
        }

        /// <summary> 导入使用的插件 </summary>
        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();

            GlobalManager.Instance.InitManager(this.AggregateCatalog);

            //导出自身程序集
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(this.GetType().Assembly));

            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(GlobalManager).Assembly));

            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(BottomModuleUC).Assembly));
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(MainModuleUC).Assembly));

            WorkClient.InitWorkClient();
        }

        protected override DependencyObject CreateShell()
        {
            try
            {
                return (DependencyObject)this.Container.GetExportedValue<IShell>();
            }
            catch (System.Exception ex)
            {
                loggerWarp.Log(ex.ToString(), Category.Exception, Priority.High);
                return null;
            }
        }

    }
}
