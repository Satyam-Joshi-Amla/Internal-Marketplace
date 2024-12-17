using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmlaMarketPlace.Models.ViewModels.Product
{
    public class CommentsViewModel
    {
        public int CommentId { get; set; }

        public int ProductId { get; set; }

        public string RejectedComments { get; set; } = null!;

        public DateTime Date { get; set; }
    }
}
