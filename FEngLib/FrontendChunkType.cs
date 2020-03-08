﻿namespace FEngLib
{
    public enum FrontendChunkType : uint
    {
        PackageHeader = 0x64486B50,
        TypeList = 0x53707954,
        ResourcesContainer = 0xCC736552,
        ResourceNames = 0x6D4E7352,
        ResourceRequests = 0x71527352,
        ObjectContainer = 0xCC6A624F,
        ButtonMapCount = 0x6E747542,
        FrontendObjectContainer = 0xEA624F46,
        ObjectData = 0x446A624F,
        ScriptData = 0x70726353,
        MessageResponses = 0x5267734D
    }
}