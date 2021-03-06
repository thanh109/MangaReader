﻿using System.Linq;
using System.Windows.Data;
using MangaReader.Core.Services;
using MangaReader.ViewModel.Commands.Primitives;
using MangaReader.ViewModel.Manga;

namespace MangaReader.ViewModel.Commands
{
  public class UpdateVisibleMangaCommand : LibraryBaseCommand
  {
    private readonly ListCollectionView view;

    public override async void Execute(object parameter)
    {
      base.Execute(parameter);

      if (Library.IsAvaible)
        await Library.ThreadAction(() => Library.Update(view.OfType<MangaModel>().Select(m => m.Id).ToList()));
    }

    public UpdateVisibleMangaCommand(ListCollectionView view, LibraryViewModel model) : base(model)
    {
      this.view = view;
      this.Name = "Обновить";
      this.Icon = "pack://application:,,,/Icons/Main/update_any.png";
    }
  }
}