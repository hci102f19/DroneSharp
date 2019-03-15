using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DroneSharp.Model.Containers
{
    public class ClusterContainer
    {
        public ClusterContainer()
        {
            Clusters = new List<Cluster>();
        }

        public List<Cluster> Clusters { get; set; }

        public Cluster GetCluster(int index)
        {
            if(Clusters.ElementAtOrDefault(index) == null)
            {
                Clusters[index] = new Cluster();
            }

            return Clusters[index];
        }

        public Cluster BestClusterAsPoint()
        {
           return Clusters.OrderBy(n => n.ClusterDensity).Reverse().FirstOrDefault();
        }
    }
}
