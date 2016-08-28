﻿using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Services;

namespace Acomics
{
  /// <summary>
  /// Манга.
  /// </summary>
  public class Acomics : MangaReader.Core.Manga.Mangas
  {
    private static Parser parser = new Parser();

    #region Свойства

    public override List<Compression.CompressionMode> AllowedCompressionModes
    {
      get
      {
        return base.AllowedCompressionModes
          .Where(m => this.HasVolumes && (Equals(m, Compression.CompressionMode.Volume) || Equals(m, Compression.CompressionMode.Chapter)) ||
            this.HasChapters && Equals(m, Compression.CompressionMode.Volume) ||
            Equals(m, Compression.CompressionMode.Manga))
          .ToList();
      }
    }

    #endregion

    #region Методы

    /// <summary>
    /// Обновить информацию о манге - название, главы, обложка.
    /// </summary>
    public override void Refresh()
    {
      parser.UpdateNameAndStatus(this);
      parser.UpdateContentType(this);
      OnPropertyChanged(nameof(IsCompleted));
    }

    protected override void UpdateContent()
    {
      this.Pages.Clear();
      this.Chapters.Clear();
      this.Volumes.Clear();

      parser.UpdateContent(this);

      base.UpdateContent();
    }

    #endregion

  }
}
