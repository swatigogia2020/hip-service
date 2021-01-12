using System.ComponentModel.DataAnnotations;

namespace In.ProjectEKA.HipService.Link.Model
{
    public class CareContextMap
    {
        public CareContextMap()
        {
        }

        public CareContextMap(
            string careContextName,
            string careContextType
        )
        {
            CareContextName = careContextName;
            CareContextType = careContextType;
        }

        public string CareContextType { get; set; }
        [Key]public string CareContextName { get; set; }
    }
}