using Internal.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Windows.Controls
{
    public class Grid : Widget
    {
        public RowDefinitionCollection RowDefinitions { set; get; }
        public ColumnDefinitionCollection ColumnDefinitions { set; get; }
        public UIElementCollection Children { set; get; }
        List<GridCollection> Grids { set; get; }
        int _rows = 1;
        int _columns = 1;

        public Grid() : base()
        {
            Children = new UIElementCollection();
            RowDefinitions = new RowDefinitionCollection();
            ColumnDefinitions = new ColumnDefinitionCollection();
            Grids = new List<GridCollection>();
        }

        public override void OnLoaded()
        {
            base.OnLoaded();

            onGrids();

            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].OnLoaded();
            }
            Debug.WriteLine($"[Grid] OnLoaded {Children.Count}");
        }

        public override void OnUnloaded()
        {
            base.OnUnloaded();

            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].OnUnloaded();
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].OnUpdate();
            }

        }

        public override void OnResize()
        {
            base.OnResize();

            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].OnResize();
            }
        }

        public override void OnDraw()
        {
            base.OnDraw();

            onGrids();
        }

        void onGrids()
        {
            if (RowDefinitions.Count == 0)
            {
                RowDefinitions.Add(new RowDefinition());
            }

            _rows = RowDefinitions.Count;

            if (ColumnDefinitions.Count == 0)
            {
                ColumnDefinitions.Add(new ColumnDefinition());
            }

            _columns = ColumnDefinitions.Count;

            if (Grids.Count == 0)
            {
                for (int c = 0; c < _columns; c++)
                {
                    for (int r = 0; r < _rows; r++)
                    {
                        Grids.Add(new GridCollection(r, c));
                    }
                }
            }
            onUpdateGrids();
            onDrawGrids();
        }

        int rowPixels = 0;
        int colPixels = 0;
        int rowTotalStar = 0;
        int colTotalStar = 0;

        void onUpdateGrids()
        {
            rowPixels = 0;
            colPixels = 0;
            rowTotalStar = 0;
            colTotalStar = 0;
         
            for (int c = 0; c < _columns; c++)
            {
                if (!ColumnDefinitions[c].Width.IsStar)
                {
                    colPixels += ColumnDefinitions[c].Width.Value;
                }
                else
                {
                    colTotalStar++;
                }
               
            }

            if (colTotalStar == 0)
            {
                colTotalStar = 1;
            }

            for (int r = 0; r < _rows; r++)
            {
                if (!RowDefinitions[r].Height.IsStar)
                {
                    rowPixels += RowDefinitions[r].Height.Value;
                }
                else
                {
                    rowTotalStar++;
                }
            }

            if (rowTotalStar == 0)
            {
                rowTotalStar = 1;
            }


            for (int c = 0; c < _columns; c++)
            {
                for (int r = 0; r < _rows; r++)
                {
                    RowDefinitions[r].Position.X = 0;
                    RowDefinitions[r].Position.Y = 0;
                    ColumnDefinitions[c].Position.X = 0;
                    ColumnDefinitions[c].Position.Y = 0;

                    GridHight(r, c);
                    GridWidth(r, c);
                    GridPosX(r, c);
                    GridPosY(r, c);
                }
            }
 
        }

        void onDrawGrids()
        {
            if (this.IsLoaded)
            {
                //Draw Controls
                for (int col = 0; col < _columns; col++)
                {
                    for (int row = 0; row < _rows; row++)
                    {
                        Position pos = new Position(ColumnDefinitions[col].Position.X, RowDefinitions[row].Position.Y, ColumnDefinitions[col].Position.Width, RowDefinitions[row].Position.Height);

                        for (int c = 0; c < Children.Count; c++)
                        {
                            if (row == Children[c].GridRow && col == Children[c].GridColumn)
                            {
                                Children[c].onSetParent(this, pos);

                                if (Children[c].GridColumnSpan > 0)
                                {
                                    Children[c].Pos.Position.Width = GetGridColumnSpan(Children[c].GridColumn, Children[c].GridColumnSpan);
                                }

                                if (Children[c].GridRowSpan > 0)
                                {
                                    Children[c].Pos.Position.Height = GetGridRowSpan(Children[c].GridRow, Children[c].GridRowSpan);
                                }

                                Children[c].OnDraw();
                            }
                        }
                    }
                }
            }
        }

        int GetGridRowSpan(int row, int span)
        {
            int result = RowDefinitions[row].Height.Value;

            if (span > _rows)
            {
                span = _rows;
            }
            for (int r = row; r < span; r++)
            {
                if (Children[r].Pos != null)
                {
                    result += RowDefinitions[r].Height.Value;
                }
            }

            return result;
        }

        int GetGridColumnSpan(int column, int span)
        {
            int result = 0;

            if (span > _columns)
            {
                span = _columns;
            }
            for (int c = column; c < span; c++)
            {
                if (Children[c].Pos != null)
                {
                    result += ColumnDefinitions[c].Width.Value;
                }
            }
          
            return result;
        }

        void GridPosX(int r, int c)
        {
            if (c == 0)
            {
                ColumnDefinitions[c].Position.X = this.X + ColumnDefinitions[c].Position.X;
            }
            else
            {
                ColumnDefinitions[c].Position.X = ColumnDefinitions[c-1].Position.X + ColumnDefinitions[c-1].Width.Value;
            }
        }

        void GridPosY(int r, int c)
        {
            if (r == 0)
            {
                RowDefinitions[r].Position.Y = this.Y + RowDefinitions[r].Position.Y;
            }
            else
            {
                RowDefinitions[r].Position.Y  = RowDefinitions[r-1].Position.Y + RowDefinitions[r-1].Height.Value;
            }
        }

        void GridHight(int r, int c)
        {
            if (RowDefinitions[r].Height.IsAuto)
            {
                RowDefinitions[r].Position.Height = RowDefinitions[r].Height.Value;
            }
            else if (RowDefinitions[r].Height.IsStar)
            {
                if (this.Parent != null)
                {
                    if (this.Parent is Grid)
                    {
                        RowDefinitions[r].Height.Value = ((((Grid)this.Parent).RowDefinitions[GridRow].Position.Height - rowPixels) / rowTotalStar);
                        RowDefinitions[r].Position.Height = RowDefinitions[r].Height.Value;
                    }
                    else
                    {
                        RowDefinitions[r].Height.Value = ((this.Parent.Height - rowPixels) / rowTotalStar);
                        RowDefinitions[r].Position.Height = RowDefinitions[r].Height.Value;
                    }
                }else
                {
                    RowDefinitions[r].Height.Value = ((this.Parent.Height - rowPixels) / rowTotalStar);
                    RowDefinitions[r].Position.Height = RowDefinitions[r].Height.Value;
                }
            }
            else if (RowDefinitions[r].Height.IsAbsolute)
            {
                RowDefinitions[r].Position.Height = RowDefinitions[r].Height.Value;
            }
        }

        void GridWidth(int r, int c)
        {
            if (ColumnDefinitions[c].Width.IsAuto)
            {
                ColumnDefinitions[c].Position.Width = ColumnDefinitions[c].Width.Value;
            }
            else if (ColumnDefinitions[c].Width.IsStar)
            {
                if (this.Parent != null)
                {
                    if (this.Parent is Grid)
                    {
                        ColumnDefinitions[c].Width.Value = ((((Grid)this.Parent).ColumnDefinitions[GridColumn].Position.Width - colPixels) / colTotalStar);
                        ColumnDefinitions[c].Position.Width = ColumnDefinitions[c].Width.Value;
                    }
                    else
                    { 
                        ColumnDefinitions[c].Width.Value = ((this.Parent.Width - colPixels) / colTotalStar);
                        ColumnDefinitions[c].Position.Width = ColumnDefinitions[c].Width.Value;
                    }
                }
                else
                {
                    ColumnDefinitions[c].Width.Value = ((this.Parent.Width - colPixels) / colTotalStar);
                    ColumnDefinitions[c].Position.Width = ColumnDefinitions[c].Width.Value;
                }
            }
            else if (ColumnDefinitions[c].Width.IsAbsolute)
            {
                ColumnDefinitions[c].Position.Width = ColumnDefinitions[c].Width.Value;
            }
        }

        public static void SetRow(Widget control, int value)
        {
            control.GridRow = value;
        }

        public static void SetColumn(Widget control, int value)
        {
            control.GridColumn = value;
        }
        public static void SetColumnSpan(Widget control, int value)
        {
            control.GridColumnSpan = value;
        }

        public  static void SetRowSpan(Widget control, int value)
        {
            control.GridRowSpan = value;
        }
    }
}
