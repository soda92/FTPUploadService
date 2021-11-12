
namespace FtpService
{

    public class DataModel
    {
        public string server_address { get; set; }
        public int port { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string local_folder { get; set; }
        public string remote_folder { get; set; }
    }
   
}
