using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord;
namespace DroneSharp.Model.Containers
{
    public class ClusterContainer
    {
        public ClusterContainer()
        {
            Clusters = new List<Cluster>();
        }

        public List<Cluster> Clusters { get; set; }

        public void Add(Cluster cluster)
        {
            Clusters.Add(cluster);
        }

        public Cluster GetCluster(int index)
        {
            if(Clusters.ElementAtOrDefault(index) == null)
            {
                Clusters[index] = new Cluster();
            }

            return Clusters[index];
        }

        public MyPoint BestClusterAsPoint()
        {
           return Clusters.OrderBy(n => n.ClusterDensity).Reverse().FirstOrDefault().GetMean();
        }
    }
}
