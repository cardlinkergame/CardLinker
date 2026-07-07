using System.Collections.Generic;

namespace CardLinker.Data
{
    /// <summary>
    /// Bir grid'e ait kartların veri koleksiyonu. "Grid" burada bir sahne değil,
    /// saf bir veri kavramı. Ana oyun grid'i de, ileride bir kartın içindeki
    /// iç grid de aynı türden birer GridContext olacak - sadece biri "root",
    /// diğeri bir CardInstanceData'nın ChildGridContextId'sine bağlı olacak.
    /// </summary>
    public class GridContext
    {
        public string Id;
        public Dictionary<string, CardInstanceData> Cards = new Dictionary<string, CardInstanceData>();
    }
}
