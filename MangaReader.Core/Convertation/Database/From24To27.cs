﻿using System;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Services;

namespace MangaReader.Core.Convertation.Database
{
  public class From24To27 : DatabaseConverter
  {
    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      RunSql(@"update Mangas 
          set HasVolumes = 1, HasChapters = 1
          where HasVolumes is null and HasChapters is null and Type = '2c98bbf4-db46-47c4-ab0e-f207e283142d'");

      RunSql(@"update Mangas 
          set HasVolumes = 0, HasChapters = 0
          where HasVolumes is null and HasChapters is null and Type = 'f090b9a2-1400-4f5e-b298-18cd35341c34'");
    }

    public From24To27()
    {
      this.Version = new Version(1, 27, 5584);
    }
  }
}