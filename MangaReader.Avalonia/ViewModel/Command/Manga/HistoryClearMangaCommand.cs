﻿using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Avalonia.Properties;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
{
  public class HistoryClearMangaCommand : MultipleMangasBaseCommand
  {
    public override void Execute(IEnumerable<IManga> mangas)
    {
      var list = mangas.ToList();

      var text = list.Count == 1 ? $"Удалить историю {list[0].Name}?" :
        ("Удалить историю?" + Environment.NewLine + string.Join(Environment.NewLine, list.Select(l => $" - {l}")));

      var dialog = new Dialogs.Avalonia.Dialog
      {
        Title = "Удаление истории",
        Description = text + Environment.NewLine + "После удаления истории манга будет скачиваться целиком."
      };
      var yes = dialog.Buttons.AddButton("Да");
      var no = dialog.Buttons.AddButton("Нет");

      if (dialog.Show() == yes)
      {
        foreach (var manga in list)
        {
          manga.ClearHistory();
        }
        list.SaveAll();
      }
    }

    public HistoryClearMangaCommand(Explorer.LibraryViewModel model) : base(model)
    {
      this.Name = Strings.Manga_Action_Remove + " историю";
      this.Icon = "pack://application:,,,/Icons/Manga/history_clear.png";
    }
  }
}