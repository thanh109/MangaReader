﻿using MangaReader.Core.Account;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.AddManga
{
  public class LogoutCommand : BaseCommand
  {
    private ILogin login;

    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && login.IsLogined;
    }

    public async override void Execute(object parameter)
    {
      base.Execute(parameter);

      await login.Logout();
    }

    public LogoutCommand(ILogin login)
    {
      this.login = login;
      this.Name = Strings.Input_Logout;
    }
  }
}