using DatingApp.Core.Domain.Entities;
using DatingApp.Core.Domain.RepositoryContracts;
using DatingApp.Core.DTO;
using DatingApp.Core.Helpers;
using DatingApp.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Infrastructure.Repositories
{

    public class MessageRepository : IMessageRepository
    {

        private ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message?> GetMessage(Guid messageId)
        {
            return await _context.Messages.FindAsync(messageId);
        }

        public async Task<PaginatedResult<MessageDto>> GetMessagesForMember(MessageParams messageParams)
        {
            var query = _context.Messages
                        .AsNoTracking()
                        .OrderByDescending(x => x.MessageSent)
                        .AsQueryable();

            query = messageParams.Container switch
            {
                MessageParams.MessageContainer.Outbox
                => query.Where(x => x.SenderId == messageParams.MemberId && x.SenderDeleted == false),
                _ => query.Where(x => x.RecipientId == messageParams.MemberId
                    && x.RecipientDeleted == false)
            };

            var messageQuery = query.Select(MessageExtensions.ToDtoProjection());

            return await PaginationHelper.CreateAsync(messageQuery, messageParams.PageNumber,
                messageParams.PageSize);
        }

        public async Task<IReadOnlyList<MessageDto>> GetMessageThread(Guid currentMemberId, Guid recipientId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.Messages
                    .Where(x => x.RecipientId == currentMemberId
                        && x.SenderId == recipientId
                        && x.DateRead == null)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.DateRead, DateTime.Now));

                var messages = await _context.Messages
                    .Where(
                    x => (x.RecipientId == currentMemberId && x.RecipientDeleted == false
                            && x.SenderId == recipientId)
                        || (x.SenderId == currentMemberId
                            && x.SenderDeleted == false
                            && x.RecipientId == recipientId))
                    .OrderBy(x => x.MessageSent)
                    .Select(MessageExtensions.ToDtoProjection())
                    .ToListAsync();

                await transaction.CommitAsync();

                return messages;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> SaveAllChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
