using System.Data.Entity;

namespace EfYou.DatabaseContext
{
    public interface ITimescaleContext
    {
        void ConfigureTimescale();
    }
}