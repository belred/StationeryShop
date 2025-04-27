using StationeryShop.Models;
using StationeryShop.Data;
using StationeryShop.DTOs.User;

namespace StationeryShop.Services
{
    public class ClientService
    {
        private readonly StationeryDbContext _db;

        public ClientService(StationeryDbContext db)
        {
            _db = db;
        }

        public async Task<ClientPublicDto?> GetClientAsync(int id)
        {
            var client = await _db.Clients.FindAsync(id);
            if (client == null)
                throw new KeyNotFoundException("Пользователь не найден");

            return new ClientPublicDto
            {
                Id = client.Id,
                Name = client.Name,
                Email = client.Email,
                Phone = client.Phone
            };
        }

        public async Task UpdateClientAsync(int id, ClientUpdateDto dto)
        {
            var client = await _db.Clients.FindAsync(id);
            if (client == null)
                throw new KeyNotFoundException("Пользователь не найден");

            if (!string.IsNullOrEmpty(dto.Name))
                client.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Phone))
            {
                if (dto.Phone.Length < 13)
                    throw new ArgumentException("Номер телефона слишком короткий");
                client.Phone = dto.Phone;
            }

            await _db.SaveChangesAsync();
        }

        public async Task DeleteClientAsync(int id)
        {
            var client = await _db.Clients.FindAsync(id);
            if (client == null || client.IsDeleted)
                throw new KeyNotFoundException("Пользователь не найден");

            client.IsDeleted = true;
            client.DeletedData = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }
    }
}