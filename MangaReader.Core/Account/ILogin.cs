﻿namespace MangaReader.Core.Account
{
  public interface ILogin : Entity.IEntity
  {
    string Name { get; set; }

    string Password { get; set; }

    bool CanLogin { get; }

    bool IsLogined { get; set; }

    System.Uri MainUri { get; set; }

    System.Uri LogoutUri { get; }

    System.Uri BookmarksUri { get; }

    System.Threading.Tasks.Task<bool> DoLogin();

    System.Threading.Tasks.Task<bool> Logout();

    System.Threading.Tasks.Task<System.Collections.Generic.List<Manga.IManga>> GetBookmarks();

    event System.EventHandler<bool> LoginStateChanged;
  }
}