﻿using System;
using System.Linq;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;

namespace Acomics.Convertation
{
  public class AcomicsFrom38To39 : ConfigConverter
  {
    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      var setting = ConfigStorage.GetPlugin<Acomics>().GetSettings();
      if (setting != null)
      {
        setting.MainUri = new Uri("https://acomics.ru/");
        setting.MangaSettingUris.Add(setting.MainUri);
        setting.Login.MainUri = setting.MainUri;
        setting.Save();
      }

      using (var context = Repository.GetEntityContext())
      {
        var mangas = context.Get<Acomics>().ToList();
        foreach (var manga in mangas)
        {
          manga.Uri = new Uri(manga.Uri.OriginalString.Replace("http://acomics.ru", "https://acomics.ru"));
          process.Percent += 100.0 / mangas.Count;
        }
        mangas.SaveAll();
      }
    }

    public AcomicsFrom38To39()
    {
      this.Version = new Version(1, 38, 6);
      this.CanReportProcess = true;
    }
  }
}