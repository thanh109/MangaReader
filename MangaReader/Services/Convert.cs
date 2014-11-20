﻿using System.Threading;

namespace MangaReader.Services
{

  enum ConverterState
  {
    None,
    Started,
    Completed
  }

  internal class ConverterProcess
  {
    internal double Percent = 0;
    internal bool IsIndeterminate = true;
    internal string Status = string.Empty;
  }

  static class Converter
  {
    public static ConverterProcess Process = new ConverterProcess();

    public static ConverterState State = ConverterState.None;

    static internal void Convert(bool withDialog)
    {
      var dialog = new Converting();
      var dialogOpened = true;
      dialog.Closed += (sender, args) => { dialogOpened = false; };

      if (withDialog)
        dialog.ShowDialog(JustConvert);
      else
        JustConvert();

      if (withDialog)
      {
        while (dialogOpened) { Thread.Sleep(1000); }
      }
    }

    static private void JustConvert()
    {
      State = ConverterState.Started;

      Process.Status = "Convert settings...";
      Settings.Convert();

      Process.Status = "Convert manga...";
      Cache.Convert(Process);

      Process.Status = "Convert history...";
      Process.Percent = 0;
      History.Convert(Process);

      Process.Status = "Convert manga list...";
      Library.Convert();

      State = ConverterState.Completed;
    }
  }
}