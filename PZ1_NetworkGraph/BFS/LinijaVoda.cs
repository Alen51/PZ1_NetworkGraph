using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ1_NetworkGraph.BFS
{
    public class LinijaVoda
    {
        private long id;
        private System.Windows.Point firstEnd;
        private System.Windows.Point secondEnd;

        public LinijaVoda()
        {

        }

        public long Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public System.Windows.Point FirstEnd { get => firstEnd; set => firstEnd = value; }
        public System.Windows.Point SecondEnd { get => secondEnd; set => secondEnd = value; }
    }
}
