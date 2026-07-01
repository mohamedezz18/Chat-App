using System;
using System.Collections.Generic;
using System.Text;
using static DatingApp.Core.Domain.RepositoryContracts.ILikesRepository;

namespace DatingApp.Core.Helpers
{
    public class LikesParams : PagingParams
    {
        public Guid MemberId { get; set; } 
        public Predicate Predicate { get; set; } = Predicate.liked;
    }
}
