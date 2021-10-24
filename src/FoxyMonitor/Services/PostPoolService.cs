using FoxyMonitor.Contracts.Services;
using FoxyMonitor.DbContexts;
using FoxyMonitor.Models;
using FoxyPoolApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FoxyMonitor.Services
{
    public class PostPoolService : ObservableObject, IPostPoolService
    {
        public ObservableCollection<string> PostPoolNames { get; private set; } = new ObservableCollection<string>();
        public ObservableCollection<PostPoolInfo> PostPools { get => _appDbContext.PostPools.Local.ToObservableCollection(); }
        public string SelectedPostPoolName
        {
            get => _selectedPostPool?.PoolApiName.ToString();
            set
            {
                var poolType = (PostPool)Enum.Parse(typeof(PostPool), value, true);
                var pool = _appDbContext.PostPools.Where(x => x.PoolApiName == poolType).FirstOrDefault();
                if (pool != null)
                {
                    SelectedPostPool = pool;
                }
            }
        }

        public PostPoolInfo SelectedPostPool { get => _selectedPostPool; set => SetProperty(ref _selectedPostPool, value); }
        private PostPoolInfo _selectedPostPool;

        private readonly AppDbContext _appDbContext;

        public PostPoolService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _appDbContext.ChangeTracker.StateChanged += ChangeTracker_StateChanged;
            _appDbContext.PostPools.Load();

            UpdatePoolNames();

            SelectedPostPoolName = "Chia_OG";
        }

        private void ChangeTracker_StateChanged(object sender, Microsoft.EntityFrameworkCore.ChangeTracking.EntityStateChangedEventArgs e)
        {
            if (e == null || e.Entry.Entity.GetType() != typeof(PostPoolInfo)) return;

            var poolItem = e.Entry.Entity as PostPoolInfo;
            // BUG: Only 'Unchanged' seem to fire, 'Added' should fire on first run when the
            // db is being populated but it does not.
            switch (e.Entry.State)
            {
                case EntityState.Detached:
                case EntityState.Unchanged:
                    // HACK: SEE BUG ABOVE
                    UpdatePoolNames();
                    return;
                case EntityState.Modified:
                    return;
                case EntityState.Deleted:
                    OnPropertyChanging(nameof(PostPoolNames));
                    PostPoolNames.Remove(poolItem.PoolApiName.ToString());
                    OnPropertyChanged(nameof(PostPoolNames));
                    return;
                case EntityState.Added:
                    OnPropertyChanging(nameof(PostPoolNames));
                    PostPoolNames.Add(poolItem.PoolApiName.ToString());
                    OnPropertyChanged(nameof(PostPoolNames));
                    return;
            }
        }

        private void UpdatePoolNames()
        {
            var postPoolApiNames = _appDbContext.PostPools.OrderBy(x => x.PoolName).Select(x => x.PoolApiName);
            foreach (var postPoolApiName in postPoolApiNames)
            {
                if (!PostPoolNames.Contains(postPoolApiName.ToString()))
                {
                    OnPropertyChanging(nameof(PostPoolNames));
                    PostPoolNames.Add(postPoolApiName.ToString());
                    OnPropertyChanged(nameof(PostPoolNames));
                }
            }

            if (postPoolApiNames.Count() == PostPoolNames.Count) return;

            var namesToRemove = new List<string>();

            foreach (var poolName in PostPoolNames)
            {
                var poolType = (PostPool)Enum.Parse(typeof(PostPool), poolName, true);
                if (!postPoolApiNames.Any(x => x == poolType))
                {
                    namesToRemove.Add(poolName);
                }
            }

            foreach (var poolName in namesToRemove)
            {
                OnPropertyChanging(nameof(PostPoolNames));
                PostPoolNames.Remove(poolName);
                OnPropertyChanged(nameof(PostPoolNames));
            }
        }
    }
}
