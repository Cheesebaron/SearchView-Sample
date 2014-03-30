using System.Collections.Generic;
using Android.App;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace SearchViewSample
{
    [Activity(Label = "SearchView Sample", MainLauncher = true, Icon = "@drawable/icon",
        Theme = "@style/Theme.AppCompat.Light")]
    public class SearchViewActivity : ActionBarActivity
    {
        private SearchView _searchView;
        private ListView _listView;
        private ChemicalsAdapter _adapter;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            SupportActionBar.SetDisplayShowHomeEnabled(true);

            var chemicals = new List<Chemical>
            {
                new Chemical {Name = "Niacin", DrawableId = Resource.Drawable.Icon},
                new Chemical {Name = "Biotin", DrawableId = Resource.Drawable.Icon},
                new Chemical {Name = "Chromichlorid", DrawableId = Resource.Drawable.Icon},
                new Chemical {Name = "Natriumselenit", DrawableId = Resource.Drawable.Icon},
                new Chemical {Name = "Manganosulfate", DrawableId = Resource.Drawable.Icon},
                new Chemical {Name = "Natriummolybdate", DrawableId = Resource.Drawable.Icon},
                new Chemical {Name = "Ergocalciferol", DrawableId = Resource.Drawable.Icon},
                new Chemical {Name = "Cyanocobalamin", DrawableId = Resource.Drawable.Icon},
            };

            _listView = FindViewById<ListView>(Resource.Id.listView);
            _adapter = new ChemicalsAdapter(this, chemicals);
            _listView.Adapter = _adapter;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main, menu);

            var item = menu.FindItem(Resource.Id.action_search);

            var searchView = MenuItemCompat.GetActionView(item);
            _searchView = searchView.JavaCast<SearchView>();

            _searchView.QueryTextChange += (s, e) => _adapter.Filter.InvokeFilter(e.NewText);

            _searchView.QueryTextSubmit += (s, e) =>
            {
                // Handle enter/search button on keyboard here
                Toast.MakeText(this, "Searched for: " + e.Query, ToastLength.Short).Show();
                e.Handled = true;
            };

            MenuItemCompat.SetOnActionExpandListener(item, new SearchViewExpandListener(_adapter));

            return true;
        }

        private class SearchViewExpandListener 
            : Java.Lang.Object, MenuItemCompat.IOnActionExpandListener
        {
            private readonly IFilterable _adapter;

            public SearchViewExpandListener(IFilterable adapter)
            {
                _adapter = adapter;
            }

            public bool OnMenuItemActionCollapse(IMenuItem item)
            {
                _adapter.Filter.InvokeFilter("");
                return true;
            }

            public bool OnMenuItemActionExpand(IMenuItem item)
            {
                return true;
            }
        }
    }
}

