using System;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;

namespace TileAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// スケジュールされたタスクを実行するエージェント
        /// </summary>
        /// <param name="task">
        /// 呼び出されたタスク
        /// </param>
        /// <remarks>
        /// このメソッドは、定期的なタスクまたはリソースを集中的に使用するタスクの呼び出し時に呼び出されます
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            var liveId = IOManagerForAgent.GetManager().LoadLivedId();
            var count = IOManagerForAgent.GetManager().LoadLiveCount();
            var manifests = IOManagerForAgent.GetManager().LoadLiveTitles();
            var checkedTitle = IOManagerForAgent.GetManager().LoadSpecified(liveId);
            var message = "";
            if (liveId.Equals(Guid.Empty))
            {
                var random = new Random();
                message = manifests[random.Next(2)];
            }
            else
            {
                message = checkedTitle;
            }

            UpdateTile(message, count);
            // If debugging is enabled, launch the agent again in one minute.
#if DEBUG
            ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(60));
#endif

            NotifyComplete();
        }

        private void UpdateTile(string title, int count)
        {
            var tile = ShellTile.ActiveTiles.FirstOrDefault();
            var newTile = new StandardTileData
            {
                Count = count,
                BackContent = title,
                BackTitle = "目標"
            };

            tile.Update(newTile);
        }
    }
}