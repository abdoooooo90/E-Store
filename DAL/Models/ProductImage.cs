using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class ProductImage : BaseEntity 
    {
        public string ImageUrl { get; set; } = null!;

        #region RS
        //Fk
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        #endregion
    }
}
