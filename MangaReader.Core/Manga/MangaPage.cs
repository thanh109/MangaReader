﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using MangaReader.Core.Exception;
using MangaReader.Core.Services;

namespace MangaReader.Core.Manga
{
  [DebuggerDisplay("{Number} {Name}")]
  public class MangaPage : Entity.Entity, IDownloadable
  {
    #region Свойства

    /// <summary>
    /// Количество перезапусков загрузки.
    /// </summary>
    private int restartCounter;

    /// <summary>
    /// Максимальное количество попыток скачивания.
    /// </summary>
    protected int MaxAttempt { get; set; }

    /// <summary>
    /// Название страницы.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Ссылка на страницу.
    /// </summary>
    public Uri Uri { get; set; }

    /// <summary>
    /// Ссылка на изображение.
    /// </summary>
    public Uri ImageLink { get; set; }

    /// <summary>
    /// Номер страницы.
    /// </summary>
    public int Number { get; set; }

    public bool IsDownloaded { get; set; }

    public double Downloaded
    {
      get { return this.IsDownloaded ? 100 : 0; }
      set { }
    }

    public string Folder { get; private set; }

    public DateTime? DownloadedAt { get; set; }

    /// <summary>
    /// Манга.
    /// </summary>
    public IManga Manga { get; set; }

    /// <summary>
    /// Глава.
    /// </summary>
    public Chapter Chapter { get; set; }

    #endregion

    #region Методы

    /// <summary>
    /// Скачать страницу.
    /// </summary>
    /// <param name="chapterFolder">Папка для файлов.</param>
    public async Task Download(string chapterFolder)
    {
      this.IsDownloaded = false;

      if (restartCounter > MaxAttempt)
        throw new DownloadAttemptFailed(restartCounter, this);

      try
      {
        await DownloadManager.CheckPause();
        using (await ThrottleService.WaitAsync())
        {
          await DownloadManager.CheckPause();
          chapterFolder = DirectoryHelpers.MakeValidPath(chapterFolder);
          if (!Directory.Exists(chapterFolder))
            Directory.CreateDirectory(chapterFolder);

          var file = await DownloadManager.DownloadImage(this.ImageLink);
          if (!file.Exist)
            OnDownloadFailed();
          var fileName = this.Number.ToString(CultureInfo.InvariantCulture).PadLeft(4, '0') + "." + file.Extension;
          await file.Save(Path.Combine(chapterFolder, fileName));
          this.IsDownloaded = true;
        }
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex, this.Uri.OriginalString);
        ++restartCounter;
        await Download(chapterFolder);
      }
    }

    protected virtual void OnDownloadFailed()
    {
      throw new System.Exception("Restart download, downloaded file is corrupted, link = " + this.ImageLink);
    }

    public void ClearHistory()
    {
      this.DownloadedAt = null;
    }

    #endregion

    #region Конструктор

    /// <summary>
    /// Глава манги.
    /// </summary>
    /// <param name="uri">Ссылка на страницу.</param>
    /// <param name="imageLink">Ссылка на изображение.</param>
    /// <param name="number">Номер страницы.</param>
    public MangaPage(Uri uri, Uri imageLink, int number)
    {
      this.Uri = uri;
      this.ImageLink = imageLink;
      this.Number = number;
      this.MaxAttempt = 3;
      this.restartCounter = 0;
    }

    protected MangaPage()
    {
      
    }

    #endregion
  }
}
