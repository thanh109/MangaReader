﻿using System;
using System.ComponentModel.Composition;
using System.Reflection;
using MangaReader.Core;

namespace Acomics
{
  [Export(typeof(IPlugin))]
  public class AcomicsPlugin : BasePlugin
  {
    public override string ShortName { get { return "AC"; } }

    public override Assembly Assembly { get {return Assembly.GetAssembly(this.GetType());} }
    public static Guid Manga { get { return Guid.Parse("F090B9A2-1400-4F5E-B298-18CD35341C34"); } }
    public override Guid MangaGuid { get { return Manga; } }
    public override Type MangaType { get { return typeof (Acomics); } }
    public override Type LoginType { get { return typeof(AcomicsLogin); } }
    public override HistoryType HistoryType { get { return HistoryType.Page; } }
    public override ISiteParser GetParser()
    {
      return new Parser();
    }
  }
}