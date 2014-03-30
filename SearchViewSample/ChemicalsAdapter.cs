using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Object = Java.Lang.Object;

namespace SearchViewSample
{
    public class ChemicalsAdapter : BaseAdapter<Chemical>, IFilterable
    {
        private List<Chemical> _originalData;
        private List<Chemical> _items; 
        private readonly Activity _context;

        public ChemicalsAdapter(Activity activity, IEnumerable<Chemical> chemicals)
        {
            _items = chemicals.OrderBy(s => s.Name).ToList();
            _context = activity;

            Filter = new ChemicalFilter(this);
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? _context.LayoutInflater.Inflate(Resource.Layout.Chemical, null);

            var chemical = _items[position];

            var nameView = view.FindViewById<TextView>(Resource.Id.chemName);
            var imageView = view.FindViewById<ImageView>(Resource.Id.chemImage);
            nameView.Text = chemical.Name;
            imageView.SetImageResource(chemical.DrawableId);

            return view;
        }

        public override int Count
        {
            get { return _items.Count; }
        }

        public override Chemical this[int position]
        {
            get { return _items[position]; }
        }

        public Filter Filter { get; private set; }

        public override void NotifyDataSetChanged()
        {
            // If you are using cool stuff like sections
            // remember to update the indices here!
            base.NotifyDataSetChanged();
        }

        private class ChemicalFilter : Filter
        {
            private readonly ChemicalsAdapter _adapter;
            public ChemicalFilter(ChemicalsAdapter adapter)
            {
                _adapter = adapter;
            }

            protected override FilterResults PerformFiltering(ICharSequence constraint)
            {
                var returnObj = new FilterResults();
                var results = new List<Chemical>();
                if (_adapter._originalData == null)
                    _adapter._originalData = _adapter._items;
                
                if (constraint == null) return returnObj;

                if (_adapter._originalData != null && _adapter._originalData.Any())
                {
                    // Compare constraint to all names lowercased. 
                    // It they are contained they are added to results.
                    results.AddRange(
                        _adapter._originalData.Where(
                            chemical => chemical.Name.ToLower().Contains(constraint.ToString())));
                }
                
                // Nasty piece of .NET to Java wrapping, be careful with this!
                returnObj.Values = FromArray(results.Select(r => r.ToJavaObject()).ToArray());
                returnObj.Count = results.Count;

                constraint.Dispose();

                return returnObj;
            }

            protected override void PublishResults(ICharSequence constraint, FilterResults results)
            {
                using (var values = results.Values)
                    _adapter._items = values.ToArray<Object>()
                        .Select(r => r.ToNetObject<Chemical>()).ToList();
                    
                _adapter.NotifyDataSetChanged();

                // Don't do this and see GREF counts rising
                constraint.Dispose();
                results.Dispose();
            }
        }
    }
}