﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MangaReader.Core.Account;
using MangaReader.Core.Manga;
using MangaReader.Core.Properties;
using MangaReader.Core.Services;

namespace Grouple
{
  public class GroupleLogin : Login
  {
    public override Uri MainUri { get; set; }
    public override Uri LogoutUri { get { return new Uri(this.MainUri, "login/logout"); } }
    public override Uri BookmarksUri { get { return new Uri(this.MainUri, "private/bookmarks"); } }

    public override async Task<bool> DoLogin()
    {
      if (IsLogined || !this.CanLogin)
        return IsLogined;

      var loginData = new NameValueCollection
      {
        {"username", this.Name},
        {"password", this.Password},
        {"remember_me", "checked"}
      };
      try
      {
        var result = await GetClient().UploadValuesTaskAsync("login/authenticate", "POST", loginData);
        IsLogined = Encoding.UTF8.GetString(result).Contains("login/logout");
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex, Strings.Login_Failed);
      }
      return IsLogined;
    }

    protected override async Task<List<IManga>> DownloadBookmarks()
    {
      var bookmarks = new List<IManga>();
      var document = new HtmlDocument();

      await this.DoLogin();

      if (!IsLogined)
        return bookmarks;

      var page = await Page.GetPageAsync(BookmarksUri, GetClient());
      document.LoadHtml(page.Content);

      var firstOrDefault = document.DocumentNode
          .SelectNodes("//div[@class=\"bookmarks-lists\"]");

      var bookMarksNode = firstOrDefault?.FirstOrDefault();
      if (bookMarksNode == null)
      {
        Log.AddFormat("Bookmarks from '{0}' not found.", this.MainUri);
        return bookmarks;
      }

      var parser = new Parser();
      var loadedBookmarks = Regex
          .Matches(bookMarksNode.OuterHtml, @"href='(.*?)'", RegexOptions.IgnoreCase)
          .OfType<Group>()
          .Select(g => g.Captures[0])
          .OfType<Match>()
          .Select(m => new Uri(m.Groups[1].Value))
          .Select(s =>
          {
            var manga = Mangas.Create(s);
            parser.UpdateNameAndStatus(manga);
            return manga;
          })
          .ToList();
      bookmarks.AddRange(loadedBookmarks);
      return bookmarks;
    }

    public GroupleLogin()
    {
      // Адрес может быть переопределен в базе. Это только дефолтное значение.
      this.MainUri = new Uri(@"https://grouple.co/");
    }
  }
}