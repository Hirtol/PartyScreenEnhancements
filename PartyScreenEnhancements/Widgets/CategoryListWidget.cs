using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using TaleWorlds.GauntletUI;

namespace PartyScreenEnhancements.Widgets
{
    public class CategoryListWidget : ListPanel
    {
        private string _tag;
        private CategoryHeaderWidget _category;


        public CategoryListWidget(UIContext context) : base(context)
        {
        }

        protected override bool OnDrop()
        {
            if (base.AcceptDrop)
            {
                bool flag = true;
                if (this.AcceptDropHandler != null)
                {
                    flag = this.AcceptDropHandler(this, base.EventManager.DraggedWidget);
                }
                if (flag)
                {
                    Widget widget = new Traverse(EventManager).Method("ReleaseDraggedWidget").GetValue() as Widget;
                    int indexForDrop = this.GetIndexForDrop(base.EventManager.DraggedWidgetPosition);
                    if (!base.DropEventHandledManually)
                    {
                        widget.ParentWidget = this;
                        widget.SetSiblingIndex(indexForDrop);
                    }
                    base.EventFired("Drop", new object[]
                    {
                        widget,
                        indexForDrop,
                        ParentCategory.Label
                    });
                    return true;
                }
            }
            return false;
        }

        public CategoryHeaderWidget ParentCategory
        {
            get => _category;
            set
            {
                if (this._category != value)
                {
                    this._category = value;
                    base.OnPropertyChanged(value, nameof(ParentCategory));
                }
            }
        }
    }
}
