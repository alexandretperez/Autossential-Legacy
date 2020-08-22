using System;
using System.Collections.Generic;
using System.Linq;

namespace Autossential.Activities.Design.Designers
{
    /// <summary>
    /// Interaction logic for AggregateDesigner.xaml
    /// </summary>
    public partial class AggregateDesigner
    {
        public AggregateDesigner()
        {
            InitializeComponent();
        }

        public List<string> AggregateFunctions
        {
            get
            {
                var type = typeof(Aggregate).GetProperty("Function").PropertyType;
                return Enum.GetNames(type).ToList();
            }
        }
    }
}