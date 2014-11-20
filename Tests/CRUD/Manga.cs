﻿using MangaReader.Manga;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Environment = Tests.Environment;

namespace MangaReader.Tests.CRUD
{
  [TestClass]
  public class Manga
  {
    [TestMethod]
    public void MangaCreateDelete()
    {
      Environment.Initialize();
      Mapping.Environment.SessionFactory = Environment.SessionFactory;


      var newManga = Builder.CreateReadmanga();
      var mangaId = newManga.Id;
      Assert.AreNotEqual(0, newManga.Id);
      using (var session = Environment.SessionFactory.OpenSession())
      {
        var fromDb = session.Get<Mangas>(mangaId);
        Assert.AreNotEqual(null, fromDb);
      }

      Builder.DeleteReadmanga(newManga);
      Assert.AreEqual(0, newManga.Id);
      using (var session = Environment.SessionFactory.OpenSession())
      {
        var fromDb = session.Get<Mangas>(mangaId);
        Assert.AreEqual(null, fromDb);
      }
    }

    [TestMethod]
    public void MangaReadUpdate()
    {
      var url = "test_url";

      Environment.Initialize();
      Mapping.Environment.SessionFactory = Environment.SessionFactory;


      var newManga = Builder.CreateAcomics();
      var oldUrl = newManga.Url;
      var mangaId = newManga.Id;

      newManga.Url = url;
      Assert.AreEqual(url, newManga.Url);

      using (var session = Environment.SessionFactory.OpenSession())
      {
        var fromDb = session.Get<Mangas>(mangaId);
        Assert.AreEqual(oldUrl, fromDb.Url);
      }
      newManga.Update();
      Assert.AreEqual(oldUrl, newManga.Url);
      using (var session = Environment.SessionFactory.OpenSession())
      {
        var fromDb = session.Get<Mangas>(mangaId);
        Assert.AreEqual(oldUrl, fromDb.Url);
      }

      newManga.Url = url;
      newManga.Save();
      Assert.AreEqual(url, newManga.Url);
      using (var session = Environment.SessionFactory.OpenSession())
      {
        var fromDb = session.Get<Mangas>(mangaId);
        Assert.AreEqual(url, fromDb.Url);
      }

      Builder.DeleteAcomics(newManga);
    }
  }
}