using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FEngViewer.WPF.UIHelpers;

/// <summary>
/// A wrapper around an <see cref="ObservableCollection{T}"/> that persists changes to an underlying list.
/// </summary>
/// <typeparam name="TModel">The underlying model type that is proxied by <typeparamref name="TViewModel"/></typeparam>
/// <typeparam name="TViewModel">The view model type that will be stored in the collection</typeparam>
public sealed class SyncingObservableCollection<TModel, TViewModel> : ObservableCollection<TViewModel>
    where TModel : class where TViewModel : ObservableObject
{
    private readonly IList<TModel> _modelList;
    private readonly Func<TModel, TViewModel> _viewModelFactory;
    private readonly Func<TViewModel, TModel> _modelExtractor;

    /// <summary>
    /// Initializes a new instance of the <see cref="SyncingObservableCollection{TModel,TViewModel}"/> class.
    /// </summary>
    /// <param name="modelList">The list</param>
    /// <param name="viewModelFactory"></param>
    /// <param name="modelExtractor"></param>
    public SyncingObservableCollection(
        IList<TModel> modelList,
        Func<TModel, TViewModel> viewModelFactory,
        Func<TViewModel, TModel> modelExtractor)
    {
        _modelList = modelList;
        _viewModelFactory = viewModelFactory;
        _modelExtractor = modelExtractor;

        // Initialize the collection with ViewModel objects
        foreach (var model in modelList)
        {
            Add(viewModelFactory(model));
        }

        CollectionChanged += OnCollectionChanged;
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                OnItemsAdded(e);
                break;
            case NotifyCollectionChangedAction.Remove:
                OnItemsRemoved(e);
                break;
            default:
                throw new Exception($"Unsupported NotifyCollectionChangedAction: {e.Action}");
        }
    }

    private void OnItemsRemoved(NotifyCollectionChangedEventArgs e)
    {
        // OldItems can't be null if we're in this method
        var oldItems = e.OldItems!;
        var lastIndex = e.OldStartingIndex + oldItems.Count - 1;

        for (int i = lastIndex; i >= e.OldStartingIndex; i--)
        {
            _modelList.RemoveAt(i);
        }
    }

    private void OnItemsAdded(NotifyCollectionChangedEventArgs e)
    {
        // NewItems can't be null if we're in this method
        var newItems = e.NewItems!;

        if (e.NewStartingIndex > _modelList.Count)
            throw new Exception($"NewStartingIndex ({e.NewStartingIndex}) is out of bounds for list of {_modelList.Count} items");

        for (var i = 0; i < newItems.Count; i++)
        {
            TViewModel newItem = (newItems[i] as TViewModel) ?? throw new Exception($"missing item {i}");
            _modelList.Insert(e.NewStartingIndex + i, _modelExtractor(newItem));
        }
    }
}