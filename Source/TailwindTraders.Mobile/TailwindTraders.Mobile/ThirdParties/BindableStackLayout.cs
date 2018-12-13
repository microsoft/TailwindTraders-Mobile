// MIT License
//
// Copyright(c) 2018 Evgeny Zborovsky
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// Based on:
// https://github.com/yuv4ik/XFBindableStackLayout/blob/master/XFBindableStackLayout/Controls/BindableStackLayout.cs

using System.Collections;
using Xamarin.Forms;

namespace XFBindableStackLayout
{
    public class BindableStackLayout : StackLayout
    {
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(BindableStackLayout),
                propertyChanged: (bindable, oldValue, newValue) => ((BindableStackLayout)bindable).PopulateItems());

        public static readonly BindableProperty ItemDataTemplateProperty =
            BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(BindableStackLayout));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemDataTemplateProperty); }
            set { SetValue(ItemDataTemplateProperty, value); }
        }

        private void PopulateItems()
        {
            Children.Clear();

            if (ItemsSource == null)
            {
                return;
            }

            foreach (var item in ItemsSource)
            {
                var itemTemplate = ItemTemplate.CreateContent() as View;
                itemTemplate.BindingContext = item;
                Children.Add(itemTemplate);
            }
        }
    }
}