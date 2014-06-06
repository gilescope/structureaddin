using StructureInterfaces;

namespace StructureAddinTests
{
    public static class Test
    {
        public static readonly AddinSettings Settings = new AddinSettings
        {
            Username = "testuser",
            Password = SecureHelper.ToSecureString("testpassword"),
            JIRAURL = "http://localhost:8080/"
        };
    }
}