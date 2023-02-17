using System;
using System.Collections.Generic;
using System.Text;

namespace Moos.Framework.Xaml.MarkupExtensions
{
    [ContentProperty(nameof(Path))]
    //[AcceptEmptyServiceProvider]
    public sealed class BindingExtension //: IMarkupExtension<BindingBase>
    {
        public string Path { get; set; } //= Binding.SelfPath;
      //  public BindingMode Mode { get; set; } = BindingMode.Default;
       // public IValueConverter Converter { get; set; }
        public object ConverterParameter { get; set; }
        public string StringFormat { get; set; }
        public object Source { get; set; }
        public string UpdateSourceEventName { get; set; }
        public object TargetNullValue { get; set; }
        public object FallbackValue { get; set; }

    }

}
