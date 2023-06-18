using System;
using Xamarin.Forms;
using DatePicker = Xamarin.Forms.DatePicker;

namespace PersonalTrainerWorkouts.Views.CustomControls;

public class NullableDatePicker : DatePicker
{
	public NullableDatePicker()
	{
		Format = "d";
	}

	public string OriginalFormat = null;
	public static readonly BindableProperty PlaceHolderProperty = GetPlaceHolderProperty();

	public static readonly BindableProperty NullableDateProperty = GetNullableDateProperty();

	private static BindableProperty GetNullableDateProperty()
	{
		return BindableProperty.Create(nameof(NullableDate)
		                             , typeof(DateTime?)
		                             , typeof(NullableDatePicker)
		                             , defaultBindingMode: BindingMode.TwoWay);
	}

	private static BindableProperty GetPlaceHolderProperty()
	{
		return BindableProperty.Create(nameof(PlaceHolder)
		                             , typeof(string)
		                             , typeof(NullableDatePicker)
		                             , " / / ");
	}

	public string PlaceHolder
	{
		get => (string)GetValue(PlaceHolderProperty);
		set => SetValue(PlaceHolderProperty
		              , value);
	}


	public DateTime? NullableDate
	{
		get => (DateTime?)GetValue(NullableDateProperty);
		set
		{
			SetValue(NullableDateProperty
			       , value);
			UpdateDate();
		}
	}

	private void UpdateDate()
	{
		if (NullableDate == null)
		{
			Format = PlaceHolder;
			return;
		}

		if (OriginalFormat != null)
		{
			Format = OriginalFormat;
		}
	}

	protected override void OnBindingContextChanged()
	{
		base.OnBindingContextChanged();

		if (BindingContext == null) return;

		OriginalFormat = Format;
		UpdateDate();
	}

	protected override void OnPropertyChanged(string propertyName = null)
	{
		base.OnPropertyChanged(propertyName);

		if (propertyName == DateProperty.PropertyName
		 || propertyName == IsFocusedProperty.PropertyName
		 && ! IsFocused
		 && Date.ToString("d") == DateTime.Now.ToString("d"))
		{
			AssignValue();
		}

		if (propertyName != NullableDateProperty.PropertyName
		 || ! NullableDate.HasValue)
		{
			return;
		}

		Date = NullableDate.Value;

		if (Date.ToString(OriginalFormat) == DateTime.Now.ToString(OriginalFormat))
		{
			//this code was done because when date selected is the actual date the"DateProperty" does not raise  
			UpdateDate();
		}
	}

	public void CleanDate()
	{
		NullableDate = null;
		UpdateDate();
	}

	public void AssignValue()
	{
		NullableDate = Date;
		UpdateDate();
	}
}