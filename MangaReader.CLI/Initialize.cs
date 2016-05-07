﻿using System;
using MangaReader.Core;
using MangaReader.Core.NHibernate;
using MangaReader.Manga;

namespace MangaReader.CLI
{
  public static class Initialize
  {
    public static void Run()
    {
      Client.Init();
      Client.Start(new ConsoleProgress());

      foreach (var manga in Repository.Get<Mangas>())
      {
        Console.WriteLine("{0}:{1}", manga.Id, manga);
      }
    }
  }
}