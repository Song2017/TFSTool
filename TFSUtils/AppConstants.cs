
namespace TFSUtils
{
    public static class AppConstants
    {
        public const string SPRINTNUM = "sprintnum";
        public const string PRONAME = "proname";
        public const string OWNERS = "owners";
        public const string CHANGESETS_ENABLE = "changesets_enable";
        public const string CHANGESETS_PROS = "changesets_pros";

        public const string TFS_USERNAME = "username";
        public const string TFS_PASSWORD = "password";
        public const string TFSURL = "tfsurl";
        public const string TFSQUERY = "tfsquery";

        public const string EMAIL_TO = "to";
        public const string EMAIL_CC = "cc";
        public const string EMAIL_SUBJECT = "subject";
        public const string EMAIL_HEADER = "emailheader";
        public const string EMAIL_FOOTER = "emailfooter";
        public const string EMAIL_SENDER = "emailsender";
    }

    public struct CryptoConstants {
        public const int PASSWORDSIZE = 32;
        public const int BLOCKSIZE = 256;
        public const int DERIVATIONITERATIONS = 1000;
        public const string ENCRYPTIONKEY = "ENCRYPTIONKEY"; 
    }
}
