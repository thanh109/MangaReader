﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MangaReader.Core.Account;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;

namespace Hentaichan
{
  public class HentaichanLogin : Login
  {
    public virtual string UserId { get; set; }

    public override string Name
    {
      get { return name; }
      set
      {
        if (name == value)
          return;

        name = value;
        this.UserId = string.Empty;
      }
    }

    public override string Password
    {
      get { return password; }
      set
      {
        if (password == value)
          return;

        password = value;
        this.PasswordHash = string.Empty;
      }
    }

    public override Uri MainUri { get; set; }
    public override Uri LogoutUri { get { return new Uri(this.MainUri, "index.php?action=logout"); } }
    public override Uri BookmarksUri { get { return new Uri(this.MainUri, "favorites/"); } }

    public virtual string PasswordHash
    {
      get
      {
        if (string.IsNullOrWhiteSpace(this.hash))
          this.hash = this.GetPasswordHash();
        return this.hash;
      }
      set { this.hash = value; }
    }

    private string hash = string.Empty;
    private string password;
    private string name;

    public virtual string GetPasswordHash()
    {
      if (string.IsNullOrWhiteSpace(this.Password))
        return string.Empty;

      using (var md5 = MD5.Create())
      {
        return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(this.Password))).Replace("-", "").ToLower();
      }
    }

    public override async Task<bool> DoLogin()
    {
      if (IsLogined || !this.CanLogin)
        return IsLogined;

      var loginData = new NameValueCollection
            {
                {"login", "submit"},
                {"login_name", this.Name},
                {"login_password", this.Password},
                {"image", "%D0%92%D1%85%D0%BE%D0%B4"}
            };

      try
      {
        await GetClient().UploadValuesTaskAsync(new Uri(MainUri, "index.php"), "POST", loginData);
        this.UserId = ClientCookie.GetCookies(this.MainUri)
            .Cast<Cookie>()
            .Single(c => c.Name == "dle_user_id")
            .Value;
        this.IsLogined = true;
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex);
        this.IsLogined = false;
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

      var pages = new List<Uri>() { BookmarksUri };

      for (int i = 0; i < pages.Count; i++)
      {
        var page = await Page.GetPageAsync(pages[i], GetClient());
        document.LoadHtml(page.Content);

        if (i == 0)
        {
          var pageNodes = document.DocumentNode.SelectNodes("//div[@class=\"navigation\"]//a");
          if (pageNodes != null)
          {
            foreach (var node in pageNodes)
            {
              pages.Add(new Uri(node.Attributes[0].Value));
            }
            pages = pages.Distinct().ToList();
          }
        }

        var nodes = document.DocumentNode.SelectNodes("//div[@class=\"manga_row1\"]");

        if (nodes == null)
        {
          Log.AddFormat("Bookmarks from '{0}' not found.", this.MainUri);
          return bookmarks;
        }

        var parser = new Parser();
        var mangas = nodes
          .Select(n => n.OuterHtml)
          .SelectMany(h => Regex.Matches(h, "href=\"(.*?)\"", RegexOptions.IgnoreCase)
                             .OfType<Group>()
                             .Select(g => g.Captures[0])
                             .OfType<Match>()
                             .Select(m => new Uri(m.Groups[1].Value.Replace("/manga/", "/related/"))))
          .Select(u =>
          {
            var manga = Mangas.Create(u);
            parser.UpdateNameAndStatus(manga);
            return manga;
          })
          .Where(m => !string.IsNullOrWhiteSpace(m.Name))
          .ToList();

        bookmarks.AddRange(mangas);
      }

      return bookmarks;
    }

    public HentaichanLogin()
    {
      // Адрес может быть переопределен в базе. Это только дефолтное значение.
      this.MainUri = new Uri(@"http://hentaichan.me/");
    }
  }
}