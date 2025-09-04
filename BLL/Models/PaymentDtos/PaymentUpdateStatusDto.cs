using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models.PaymentDtos
{
    public class PaymentUpdateStatusDto
    {
        public int PaymentId { get; set; }
        public string Status { get; set; } = null!;
    }
}
