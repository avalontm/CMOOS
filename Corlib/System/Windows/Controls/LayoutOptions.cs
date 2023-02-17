using System;

namespace System.Windows.Controls
{
    //
    // Resumen:
    //     A struct whose static members define various alignment and expansion options.
    //
    // Comentarios:
    //     To be added.
    //[TypeConverter(typeof(LayoutOptionsConverter))]
    public struct LayoutOptions
    {
        //
        // Resumen:
        //     A Microsoft.Maui.Controls.LayoutOptions structure that describes an element that
        //     appears at the start of its parent and does not expand.
        //
        // Comentarios:
        //     To be added.
        public static readonly LayoutOptions Start;
        //
        // Resumen:
        //     A Microsoft.Maui.Controls.LayoutOptions structure that describes an element that
        //     is centered and does not expand.
        //
        // Comentarios:
        //     To be added.
        public static readonly LayoutOptions Center;
        //
        // Resumen:
        //     A Microsoft.Maui.Controls.LayoutOptions structure that describes an element that
        //     appears at the end of its parent and does not expand.
        //
        // Comentarios:
        //     To be added.
        public static readonly LayoutOptions End;
        //
        // Resumen:
        //     A Microsoft.Maui.Controls.LayoutOptions stucture that describes an element that
        //     has no padding around itself and does not expand.
        //
        // Comentarios:
        //     To be added.
        public static readonly LayoutOptions Fill;
        //
        // Resumen:
        //     Creates a new Microsoft.Maui.Controls.LayoutOptions object with alignment and
        //     expands.
        //
        // Parámetros:
        //   alignment:
        //     An alignment value.
        //
        //   expands:
        //     Whether or not an element will expand to fill available space in its parent.
        //
        // Comentarios:
        //     To be added.
        public LayoutOptions(LayoutAlignment alignment, bool expands)
        {
            Alignment = alignment;
            Expands = expands;
        }

        //
        // Resumen:
        //     Gets or sets a value that indicates how an element will be aligned.
        //
        // Valor:
        //     The Microsoft.Maui.Controls.LayoutAlignment flags that describe the behavior
        //     of an element.
        //
        // Comentarios:
        //     To be added.
        public LayoutAlignment Alignment { get; set; }
        //
        // Resumen:
        //     Gets or sets a value that indicates whether or not the element that is described
        //     by this Microsoft.Maui.Controls.LayoutOptions structure will occupy the largest
        //     space that its parent will give to it.
        //
        // Valor:
        //     Whether or not the element that is described by this Microsoft.Maui.Controls.LayoutOptions
        //     structure will occupy the largest space that its parent will give it. true if
        //     the element will occupy the largest space the parent will give to it. false if
        //     the element will be as compact as it can be.
        //
        // Comentarios:
        //     To be added.
        public bool Expands { get; set; }
    }
}
