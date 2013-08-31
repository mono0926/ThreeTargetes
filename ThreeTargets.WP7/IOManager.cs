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
using Mono.App.ThreeTargets.Model;
using Mono.App.ThreeTargets.ViewModel;

namespace Mono.App.ThreeTargets
{
    public class IOManager
    {
        private static IOManager manager = new IOManager();

        public static IOManager GetManager()
        {
            return manager;
        }

        public void WriteInitial(ManifestModel first, ManifestModel second, ManifestModel third)
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var stream = isoStore.OpenFile("data.xml", System.IO.FileMode.Create))
                {
                    var root = new XElement("root");

                    root.Add(GenerateManifest(first));
                    root.Add(GenerateManifest(second));
                    root.Add(GenerateManifest(third));
                    var app = App.Current as App;
                    app.Manifests = new List<ManifestModel>();
                    app.Manifests.Add(third);
                    app.Manifests.Add(second);
                    app.Manifests.Add(first);
                    //app.SelectedManifestID = first.ID;

                    root.Save(stream);
                }
            }
        }

        public void UpdateManifest(ManifestModel manifest)
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                XElement root = null;
                using (var stream = isoStore.OpenFile("data.xml", System.IO.FileMode.Open))
                {
                    root = XElement.Load(stream);
                    var removeTarget = root.Elements("manifest").Where(a => a.Attribute("ID").Value.Equals(manifest.ID.ToString())).FirstOrDefault();
                    removeTarget.Remove();
                    root.Add(GenerateManifest(manifest, true));
                }
                using (var stream = isoStore.OpenFile("data.xml", System.IO.FileMode.Create))
                {
                    (App.Current as App).UpdateManifests(manifest);
                    root.Save(stream);
                }
            }
        }

        private XElement GenerateManifest(ManifestModel manifest, bool isUpdate = false)
        {
            var doc = new XElement("manifest",
                        new XElement("title", manifest.Title),
                        new XElement("isDone", manifest.IsDone));

            var guid = Guid.NewGuid();
            if (isUpdate)
            {
                guid = manifest.ID;
            }
            doc.SetAttributeValue("ID", guid);

            var contentsDoc = new XElement("contents");
            foreach (var c in manifest.Contents)
            {
                contentsDoc.Add(new XElement("content",
                    new XElement("description", c.Description),
                    new XElement("detail", c.Detail)));
            }

            if (manifest.Contents.Count > 0)
            {
                doc.Add(contentsDoc);
            }
            return doc;
        }

        public IList<ManifestModel> LoadManifests(bool isSave = true)
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var stream = isoStore.OpenFile("data.xml", System.IO.FileMode.OpenOrCreate))
                {
                    return LoadManifests(stream, isSave);
                }
            }
        }

        public ManifestModel LoadManifest(Guid ID)
        {
            return ((App.Current as App).Manifests.Where(m => m.ID.Equals(ID))).FirstOrDefault();
        }

        private IList<ManifestModel> LoadManifests(IsolatedStorageFileStream stream, bool isSave)
        {
            try
            {
                var doc = XDocument.Load(stream);
                var result = (from m in doc.Descendants("manifest")
                              select new ManifestModel()
                              {
                                  ID = Guid.Parse(m.Attribute("ID").Value),
                                  Title = m.Element("title").Value,
                                  IsDone = bool.Parse(m.Element("isDone").Value),
                                  Contents = (from c in m.Descendants("content")
                                              select new ContentModel()
                                              {
                                                  Description = c.Element("description").Value ?? "",
                                                  Detail = c.Element("detail").Value ?? "",
                                              }).ToList()
                              }).ToList();
                if (isSave)
                {
                    (App.Current as App).Manifests = result;
                }
                return result;
            }
            catch (Exception)
            {
                return new List<ManifestModel>();
            }
        }

        internal void SaveSetting(SettingViewModel settingViewModel)
        {
            var setting = IsolatedStorageSettings.ApplicationSettings;
            setting["isLiveChecked"] = settingViewModel.IsLiveChecked;
            var id = (from i in settingViewModel.Items where i.IsChecked select i.ID).FirstOrDefault();
            setting["liveID"] = id;
            (App.Current as App).Setting = new Setting { ID = id, IsLiveChecked = settingViewModel.IsLiveChecked };
            setting.Save();
        }

        internal Setting LoadSetting()
        {
            var s = LoadFromAgent();
            (App.Current as App).Setting = s;
            return s;
        }

        internal Setting LoadFromAgent()
        {
            var setting = IsolatedStorageSettings.ApplicationSettings;
            var s = new Setting();
            if (setting.Contains("isLiveChecked"))
            {
                s.IsLiveChecked = (bool)setting["isLiveChecked"];
            }
            else
            {
                s.IsLiveChecked = true;
            }
            if (setting.Contains("liveID"))
            {
                s.ID = (Guid)setting["liveID"];
            }
            else
            {
                s.ID = Guid.Empty;
            }
            return s;
        }

        internal void DeleteAll()
        {
            (App.Current as App).Setting = new Setting();
            (App.Current as App).SelectedManifestID = Guid.Empty;
            (App.Current as App).Manifests = null;
            var setting = IsolatedStorageSettings.ApplicationSettings;
            setting.Remove("isLiveChecked");
            setting.Remove("liveID");
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                isoStore.Remove();
            }
        }
    }
}