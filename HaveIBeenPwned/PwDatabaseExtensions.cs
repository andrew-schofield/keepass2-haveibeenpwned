using KeePass.Plugins;
using KeePassLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaveIBeenPwned
{
    public static class PwDatabaseExtensions
    {
        public static void MarkAsModified(this IPluginHost pluginHost)
        {
            // Make a nothing change to try to trigger the '*' to notify the user of a change.

            // TODO: This doesn't seem to work properly. Need to figure out why and fix it.

            var database = pluginHost.Database;

            // And This doesn't seem to work
            database.Modified = true;

            var notes = database.RootGroup.Notes;
            database.RootGroup.Notes = notes + ".";
            database.RootGroup.Notes = notes;
            
            pluginHost.MainWindow.Refresh();
        }
    }
}
