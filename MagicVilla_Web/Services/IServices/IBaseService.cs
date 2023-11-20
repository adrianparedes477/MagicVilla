using MagicVilla_Web.Models;

namespace MagicVilla_Web.Services.IServices
{
    public interface IBaseService
    {
        public APIResponse resposeModel { get; set; }

        Task<T> SeedAsync<T>(APIRequest apiRequest);
    }
}
