﻿using System;
using System.Diagnostics;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands
{
  public class NavigationFromHistory : BaseCommand
  {
    private const string RepoUri = "https://github.com/MonkAlex/MangaReader/path_to_remove";

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      var uri = new Uri($"{RepoUri}{parameter}");
      Process.Start(uri.ToString());
    }
  }
}