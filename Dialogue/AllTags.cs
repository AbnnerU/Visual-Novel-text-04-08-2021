
using UnityEngine;

public static class AllTags 
{
    public static string[] mainTags = new string[] {
        "[BEGIN]", "[END]", "[N]", "[S]",
        "[ANSWERS]", "[OPTIONS]", "[OPT1]", "[OPT2]",
        "[OPT3]", "[OPT4]", "[SKIP]", "[SKIP*]"
    };

    public static string[] eventsTag = new string[] { "[IMAGEEVENT]","[SOUNDEVENT]","[ANIMATIONEVENT]" };

    public static string[] systemTags = new string[] { "[UNLOCK]", "[SAVE]" };      

    public static string[] optionsTag = new string[] { "[OPT1]", "[OPT2]", "[OPT3]", "[OPT4]" };

    //public static string[] logicalTagsBegin = new string[]{ "<LOCK>" };

    //public static string[] logicalTagsEnd = new string[] { "</LOCK>" };

    public static string[] animationType = new string[] { "WAVE", "RAINBOW" };

}
