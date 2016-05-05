using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lisb.Entity;
using Lisb.Repository;

namespace ChatRoomSignalR
{
    public partial class PinterrestByMasonry : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Repeater1.DataSource = ProductRepository.GetData(1, 25);
            Repeater1.DataBind();
        }

        [WebMethod]
        public static IEnumerable<Product> GetData(int pageNo, int pageSize)
        {

            return ProductRepository.GetData(pageNo, pageSize);
        }
    }
}