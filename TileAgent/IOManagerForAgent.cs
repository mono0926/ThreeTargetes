using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace TileAgent
{
    public class IOManagerForAgent
    {
        private static IOManagerForAgent manager = new IOManagerForAgent();

        public static IOManagerForAgent GetManager()
        {
            return manager;
        }

        public int LoadLiveCount()
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var stream = isoStore.OpenFile("data.xml", System.IO.FileMode.OpenOrCreate))
                {
                    var doc = XDocument.Load(stream);
                    var result = (from m in doc.Descendants("manifest")
                                  where !bool.Parse(m.Element("isDone").Value)
                                  select m).Count();
                    return result;
                }
            }
        }

        public IList<string> LoadLiveTitles()
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var stream = isoStore.OpenFile("data.xml", System.IO.FileMode.OpenOrCreate))
                {
                    var doc = XDocument.Load(stream);
                    var result = (from m in doc.Descendants("manifest")
                                  select m.Element("title").Value).ToList();
                    return result;
                }
            }
        }

        public string LoadSpecified(Guid id)
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var stream = isoStore.OpenFile("data.xml", System.IO.FileMode.OpenOrCreate))
                {
                    var doc = XDocument.Load(stream);

                    var result = (from m in doc.Descendants("manifest")
                                  let i = Guid.Parse(m.Attribute("ID").Value)
                                  where i.Equals(id)
                                  select m.Element("title").Value).FirstOrDefault();
                    return result;
                }
            }
        }

        internal Guid LoadLivedId()
        {
            var setting = IsolatedStorageSettings.ApplicationSettings;
            var id = Guid.Empty;
            if (setting.Contains("liveID"))
            {
                id = (Guid)setting["liveID"];
            }
            return id;
        }
    }
}