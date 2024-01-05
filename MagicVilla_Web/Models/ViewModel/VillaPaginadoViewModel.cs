using MagicVilla_Web.Models.Dto;

namespace MagicVilla_Web.Models.ViewModel
{
    public class VillaPaginadoViewModel
    {
        public int PageNumber { get; set; }

        public int TotalPaginas { get; set; }

        public string Previo { get; set; } = "disable";

        public string Siguiente { get; set; } = "null";

        public IEnumerable<VillaDto> VillaList { get; set; }
    }
}
