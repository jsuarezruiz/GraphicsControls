﻿using System;
using System.Collections.Generic;
using System.Graphics;
using System.Runtime.CompilerServices;
using GraphicsControls.Effects;
using Xamarin.Forms;
using XColor = Xamarin.Forms.Color;

namespace GraphicsControls
{
    public partial class DatePicker : GraphicsVisualView
    {
        DatePickerDialogRoutingEffect _datePickerEffect;

        public static class Layers
        {
            public const string Background = "DatePicker.Layers.Background";
            public const string Border = "DatePicker.Layers.Border";
            public const string Icon = "DatePicker.Layers.Icon";
            public const string Placeholder = "DatePicker.Layers.Placeholder";
            public const string Date = "DatePicker.Layers.Date";
        }

        public static readonly BindableProperty DateProperty =
            BindableProperty.Create(nameof(Date), typeof(DateTime), typeof(DatePicker), default(DateTime), BindingMode.TwoWay,
                coerceValue: CoerceDate,
                propertyChanged: DatePropertyChanged,
                defaultValueCreator: (bindable) => DateTime.Today);

        public static readonly BindableProperty MinimumDateProperty =
            BindableProperty.Create(nameof(MinimumDate), typeof(DateTime), typeof(DatePicker), new DateTime(1900, 1, 1),
                validateValue: ValidateMinimumDate, coerceValue: CoerceMinimumDate);

        public static readonly BindableProperty MaximumDateProperty =
            BindableProperty.Create(nameof(MaximumDate), typeof(DateTime), typeof(DatePicker), new DateTime(2100, 12, 31),
                validateValue: ValidateMaximumDate, coerceValue: CoerceMaximumDate);

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create(nameof(TextColor), typeof(XColor), typeof(TimePicker), XColor.Default);

        static object CoerceDate(BindableObject bindable, object value)
        {
            var picker = (DatePicker)bindable;
            DateTime dateValue = ((DateTime)value).Date;

            if (dateValue > picker.MaximumDate)
                dateValue = picker.MaximumDate;

            if (dateValue < picker.MinimumDate)
                dateValue = picker.MinimumDate;

            return dateValue;
        }

        static void DatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var datePicker = (DatePicker)bindable;
            datePicker.DateSelected?.Invoke(datePicker, new DateChangedEventArgs((DateTime)oldValue, (DateTime)newValue));
        }

        static bool ValidateMinimumDate(BindableObject bindable, object value)
        {
            return ((DateTime)value).Date <= ((DatePicker)bindable).MaximumDate.Date;
        }

        static object CoerceMinimumDate(BindableObject bindable, object value)
        {
            DateTime dateValue = ((DateTime)value).Date;
            var picker = (DatePicker)bindable;
            if (picker.Date < dateValue)
                picker.Date = dateValue;

            return dateValue;
        }

        static bool ValidateMaximumDate(BindableObject bindable, object value)
        {
            return ((DateTime)value).Date >= ((DatePicker)bindable).MinimumDate.Date;
        }

        static object CoerceMaximumDate(BindableObject bindable, object value)
        {
            DateTime dateValue = ((DateTime)value).Date;
            var picker = (DatePicker)bindable;
            if (picker.Date > dateValue)
                picker.Date = dateValue;

            return dateValue;
        }

        public DateTime Date
        {
            get { return (DateTime)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }

        public DateTime MaximumDate
        {
            get { return (DateTime)GetValue(MaximumDateProperty); }
            set { SetValue(MaximumDateProperty, value); }
        }

        public DateTime MinimumDate
        {
            get { return (DateTime)GetValue(MinimumDateProperty); }
            set { SetValue(MinimumDateProperty, value); }
        }

        public XColor TextColor
        {
            get { return (XColor)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public List<string> DatePickerLayers = new List<string>
        {
            Layers.Background,
            Layers.Border,
            Layers.Icon,
            Layers.Placeholder,
            Layers.Date
        };

        public event EventHandler<DateChangedEventArgs> DateSelected;

        public override void Load()
        {
            base.Load();

            switch (VisualType)
            {
                case VisualType.Material:
                default:
                    HeightRequest = 56;
                    break;
                case VisualType.Cupertino:
                    HeightRequest = 36;
                    break;
                case VisualType.Fluent:
                    HeightRequest = 32;
                    break;
            }

            _datePickerEffect = new DatePickerDialogRoutingEffect();

            Effects.Add(_datePickerEffect);

            UpdateMaximumDate();
            UpdateMinimumDate();
        }

        public override void Unload()
        {
            Effects.Remove(_datePickerEffect);
        }

        public override List<string> GraphicsLayers =>
            DatePickerLayers;

        public override void DrawLayer(string layer, ICanvas canvas, RectangleF dirtyRect)
        {
            switch (layer)
            {
                case Layers.Background:
                    DrawDatePickerBackground(canvas, dirtyRect);
                    break;
                case Layers.Border:
                    DrawDatePickerBorder(canvas, dirtyRect);
                    break;
                case Layers.Icon:
                    DrawDatePickerIcon(canvas, dirtyRect);
                    break;
                case Layers.Placeholder:
                    DrawDatePickerPlaceholder(canvas, dirtyRect);
                    break;
                case Layers.Date:
                    DrawDatePickerDate(canvas, dirtyRect);
                    break;
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == MaximumDateProperty.PropertyName)
                UpdateMaximumDate();
            else if (propertyName == MinimumDateProperty.PropertyName)
                UpdateMinimumDate();
        }

        protected virtual void DrawDatePickerBackground(ICanvas canvas, RectangleF dirtyRect)
        {
            switch (VisualType)
            {
                case VisualType.Material:
                default:
                    DrawMaterialDatePickerBackground(canvas, dirtyRect);
                    break;
                case VisualType.Cupertino:
                    DrawCupertinoDatePickerBackground(canvas, dirtyRect);
                    break;
                case VisualType.Fluent:
                    DrawFluentDatePickerBackground(canvas, dirtyRect);
                    break;
            }
        }

        protected virtual void DrawDatePickerBorder(ICanvas canvas, RectangleF dirtyRect)
        {
            switch (VisualType)
            {
                case VisualType.Material:
                default:
                    DrawMaterialDatePickerBorder(canvas, dirtyRect);
                    break;
                case VisualType.Cupertino:
                    DrawCupertinoDatePickerBorder(canvas, dirtyRect);
                    break;
                case VisualType.Fluent:
                    DrawFluentDatePickerBorder(canvas, dirtyRect);
                    break;
            }
        }

        protected virtual void DrawDatePickerIcon(ICanvas canvas, RectangleF dirtyRect)
        {
            switch (VisualType)
            {
                case VisualType.Fluent:
                    DrawFluentDatePickerIcon(canvas, dirtyRect);
                    break;
            }
        }

        protected virtual void DrawDatePickerPlaceholder(ICanvas canvas, RectangleF dirtyRect)
        {
            switch (VisualType)
            {
                case VisualType.Material:
                    DrawMaterialDatePickerPlaceholder(canvas, dirtyRect);
                    break;
            }
        }

        protected virtual void DrawDatePickerDate(ICanvas canvas, RectangleF dirtyRect)
        {
            switch (VisualType)
            {
                case VisualType.Material:
                default:
                    DrawMaterialDatePickerDate(canvas, dirtyRect);
                    break;
                case VisualType.Cupertino:
                    DrawCupertinoDatePickerDate(canvas, dirtyRect);
                    break;
                case VisualType.Fluent:
                    DrawFluentDatePickerDate(canvas, dirtyRect);
                    break;
            }
        }

        void UpdateMaximumDate()
        {
            DatePickerDialog.SetMaximumDate(this, MaximumDate);
        }

        void UpdateMinimumDate()
        {
            DatePickerDialog.SetMinimumDate(this, MinimumDate);
        }

        DateTime GetDate()
        {
            var date = DatePickerDialog.GetDate(this);

            if (date == default)
                return MinimumDate;

            return date;
        }
    }
}