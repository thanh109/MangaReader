using System;
using System.Linq;
using MangaReader.Core.Manga;
using NUnit.Framework;
using MangaReader.Core.Services.Config;

namespace Tests.Entities.Manga
{
  [TestFixture]
  public class MangachanStructure : TestClass
  {
    [Test]
    public void AddMangachanMultiPages()
    {
      var manga = GetManga("http://mangachan.me/manga/3828-12-prince.html");
      new Hentaichan.Mangachan.Parser().UpdateContent(manga);
      Assert.AreEqual(16, manga.Volumes.Count);
      Assert.AreEqual(78, manga.Volumes.Sum(v => v.Container.Count()));
    }

    [Test]
    public void AddMangachanSingleChapter()
    {
      var manga = GetManga("http://mangachan.me/manga/20138-16000-honesty.html");
      new Hentaichan.Mangachan.Parser().UpdateContent(manga);
      Assert.AreEqual(1, manga.Volumes.Count);
      Assert.AreEqual(1, manga.Volumes.Sum(v => v.Container.Count()));
    }

    private static IManga GetManga(string url)
    {
      return Mangas.CreateFromWeb(new Uri(url));
    }

    [Test]
    public void MangachanNameParsing()
    {
      // Спецсимвол "
      TestNameParsing("http://mangachan.me/manga/48069-isekai-de-kuro-no-iyashi-te-tte-yobarete-imasu.html",
        "Isekai de \"Kuro no Iyashi Te\" tte Yobarete Imasu",
        "В другом мире моё имя - Чёрный целитель");

      // Просто проверка.
      TestNameParsing("http://mangachan.me/manga/46475-shin5-kekkonshite-mo-koishiteru.html",
        "#shin5 - Kekkonshite mo Koishiteru",
        "Любовь после свадьбы");

      // Нет русского варианта.
      TestNameParsing("http://mangachan.me/manga/17625--okazaki-mari.html",
        "& (Okazaki Mari)",
        "& (Okazaki Mari)");
      
      // Символ звездочки *
      TestNameParsing("http://mangachan.me/manga/23099--asterisk.html",
        "* - Asterisk",
        "Звездочка");
    }

    private void TestNameParsing(string uri, string english, string russian)
    {
      ConfigStorage.Instance.AppConfig.Language = Languages.English;
      var manga = GetManga(uri);
      Assert.AreEqual(english, manga.Name);
      ConfigStorage.Instance.AppConfig.Language = Languages.Russian;
      new Hentaichan.Mangachan.Parser().UpdateNameAndStatus(manga);
      Assert.AreEqual(russian, manga.Name);
    }
  }
}
