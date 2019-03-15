using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneSharp.Model.DBSCAN
{
    public class DbScanPoint<T>
    {
        public bool IsVisited;
        public T ClusterPoint;
        public int ClusterId;

        public DbScanPoint(T x)
        {
            ClusterPoint = x;
            IsVisited = false;
            ClusterId = (int)ClusterIds.Unclassified;
        }

    }
}
