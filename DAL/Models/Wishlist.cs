using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Wishlist : BaseEntity
    {
        public string UserId { get; set; } = null!;
        public int ProductId { get; set; }

        #region Relationships
        public ApplicationUser User { get; set; } = null!;
        public Product Product { get; set; } = null!;
        #endregion
    }
}
