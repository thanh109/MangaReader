﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ReactiveUI;

namespace MangaReader.Avalonia.ViewModel
{
  public class ViewModelBase : ReactiveObject
  {
    protected bool RaiseAndSetIfChanged<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
      if (!EqualityComparer<T>.Default.Equals(field, value))
      {
        field = value;
        (this as IReactiveObject)?.RaisePropertyChanged(new PropertyChangedEventArgs(propertyName));
        return true;
      }

      return false;
    }

    protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
    {
      (this as IReactiveObject)?.RaisePropertyChanged(new PropertyChangedEventArgs(propertyName));
    }
  }
}