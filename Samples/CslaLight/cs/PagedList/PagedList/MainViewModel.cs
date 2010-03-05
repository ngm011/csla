﻿using System;
using System.Windows;
using Csla.Xaml;

namespace PagedList
{
  public class MainViewModel : ViewModel<Library.DataList>
  {
    public MainViewModel()
    {
      // set to true for automatic paged loading, false to use the Next button
      AutoLoad = false;

      if (AutoLoad)
        BeginRefresh("GetListPaged");
      else
        BeginRefresh("GetFirstPage");
    }

    public static readonly DependencyProperty AutoLoadProperty =
      DependencyProperty.Register("AutoLoad", typeof(bool), typeof(MainViewModel), null);
    public bool AutoLoad
    {
      get { return (bool)GetValue(AutoLoadProperty); }
      set { SetValue(AutoLoadProperty, value); }
    }

    public static readonly DependencyProperty LoadedRowCountProperty =
      DependencyProperty.Register("LoadedRowCount", typeof(int), typeof(MainViewModel), null);
    public int LoadedRowCount
    {
      get { return (int)GetValue(LoadedRowCountProperty); }
      set { SetValue(LoadedRowCountProperty, value); }
    }

    public static readonly DependencyProperty TotalRowCountProperty =
      DependencyProperty.Register("TotalRowCount", typeof(int), typeof(MainViewModel), null);
    public int TotalRowCount
    {
      get { return (int)GetValue(TotalRowCountProperty); }
      set { SetValue(TotalRowCountProperty, value); }
    }
   
    protected override void OnRefreshed()
    {
      TotalRowCount = Model.TotalRowCount;
      LoadedRowCount = Model.Count;
      Model.CollectionChanged += (o, e) => LoadedRowCount = Model.Count;
      base.OnRefreshed();
    }

    private int _lastPage;

    public void NextPage()
    {
      if (AutoLoad)
        throw new InvalidOperationException("NextPage");

      _lastPage++;
      Library.DataList.GetPage(_lastPage, (o, e) =>
        {
          if (e.Error == null)
            Model.AddRange(e.Object);
        });
    }
  }
}
