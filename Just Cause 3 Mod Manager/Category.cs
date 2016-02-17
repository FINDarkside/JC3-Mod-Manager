using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Just_Cause_3_Mod_Manager
{
	public class Category : INotifyPropertyChanged
	{
		public string Name { get; set; }
		private bool selected;
		public bool Selected
		{
			get { return selected; }
			set {
				SetPropertyField(ref selected, value); 
			}
		}

		private int modCount;
		[JsonIgnore]
		public int ModCount
		{
			get { return modCount; }
			set { SetPropertyField(ref modCount, value); }
		}

		public Category(string name)
		{
			Name = name;
		}

		public override string ToString()
		{
			return Name;
		}

		protected void SetPropertyField<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
		{
			if (!EqualityComparer<T>.Default.Equals(field, newValue))
			{
				field = newValue;
				PropertyChangedEventHandler handler = PropertyChanged;
				if (handler != null)
					handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
